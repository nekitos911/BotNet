using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BotNet.Services.SystemOs
{
    class SystemService
    {
        public string RegGet(string key, string value)
        {
            var registryKey = Registry.LocalMachine.OpenSubKey(key, false);
            if (registryKey != null)
            {
                object rkey = registryKey.GetValue(value, (object)(string)"noValueButYesKey");
                if (rkey is string)
                {
                    return rkey.ToString();
                }
                if (registryKey.GetValueKind(value) == RegistryValueKind.String || registryKey.GetValueKind(value) == RegistryValueKind.ExpandString)
                {
                    return rkey.ToString();
                }
                if (registryKey.GetValueKind(value) == RegistryValueKind.DWord)
                {
                    return Convert.ToString((Int32)rkey);
                }
                if (registryKey.GetValueKind(value) == RegistryValueKind.QWord)
                {
                    return Convert.ToString((Int64)rkey);
                }
                if (registryKey.GetValueKind(value) == RegistryValueKind.Binary)
                {
                    return Convert.ToString((byte[])rkey);
                }
                if (registryKey.GetValueKind(value) == RegistryValueKind.MultiString)
                {
                    return string.Join("", (string[])rkey);
                }
                return "noValueButYesKey";
            }

            return "noKey";
        }
    }
}
