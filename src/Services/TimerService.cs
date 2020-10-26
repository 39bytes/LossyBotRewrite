using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Xml.Linq;

namespace LossyBotRewrite
{
    public class TimerService
    {
        private readonly DiscordSocketClient _client;

        private Timer timer;
        public TimerService(DiscordSocketClient client)
        {
            _client = client;

            timer = new Timer();
            timer.Interval = 60000;
            timer.Elapsed += CheckPrivate;
            timer.Start();
        }

        private void CheckPrivate(object sender, ElapsedEventArgs e)
        {
            XDocument doc = XDocument.Load("Servers.xml");

        }
    }
}
