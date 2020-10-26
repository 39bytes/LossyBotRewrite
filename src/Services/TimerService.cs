using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private async void CheckPrivate(object sender, ElapsedEventArgs e)
        {
            XDocument doc = XDocument.Load(Globals.path + "Servers.xml");
            var privateChannels = doc.Root.Descendants("PrivateChannel").Where(elem => elem.Value != "");
            foreach(var channel in privateChannels)
            {
                var textChannel = (_client.GetChannel(ulong.Parse(channel.Value)) as SocketTextChannel);

                IEnumerable<IMessage> messages = await textChannel.GetMessagesAsync(1000).FlattenAsync();
                List<IMessage> trashCan = new List<IMessage>();
                foreach(IMessage message in messages)
                {
                    if (DateTimeOffset.UtcNow > message.Timestamp + TimeSpan.FromHours(1))
                    {
                        trashCan.Add(message);
                    }
                }
                await textChannel.DeleteMessagesAsync(trashCan.AsEnumerable());
            }
        }
    }
}
