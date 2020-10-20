using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Tweetinvi.Models.DTO;

namespace LossyBotRewrite
{
    public class VoiceStateService
    {
        private readonly DiscordSocketClient _client;
        private readonly VoiceServiceManager _voiceManager;

        XDocument doc = XDocument.Load(Globals.path + "Servers.xml");

        public VoiceStateService(DiscordSocketClient client, VoiceServiceManager manager)
        {
            _client = client;
            _voiceManager = manager;

            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;
        }

        private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (oldState.VoiceChannel == null && newState.VoiceChannel != null)
            {
                XElement roleElem = doc.Root.XPathSelectElement($"./server[@id='{newState.VoiceChannel.Guild.Id}']/InVoiceRole");
                if(roleElem.Value == null)
                    return;

                var role = newState.VoiceChannel.Guild.GetRole(ulong.Parse(roleElem.Value));
                await (user as IGuildUser).AddRoleAsync(role);
            }
            else if(oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                XElement roleElem = doc.Root.XPathSelectElement($"./server[@id='{oldState.VoiceChannel.Guild.Id}']/InVoiceRole");
                if (roleElem.Value == null)
                    return;

                var role = oldState.VoiceChannel.Guild.GetRole(ulong.Parse(roleElem.Value));
                await (user as IGuildUser).RemoveRoleAsync(role);
            }
        }
    }
}
