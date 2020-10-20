using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LossyBotRewrite
{
    public class VoiceStateService
    {
        private readonly DiscordSocketClient _client;
        private readonly VoiceServiceManager _voiceManager;

        public VoiceStateService(DiscordSocketClient client, VoiceServiceManager manager)
        {
            _client = client;
            _voiceManager = manager;

            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;
        }

        private async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            XDocument doc = XDocument.Load(Globals.path + "Servers.xml");
            if (oldState.VoiceChannel == null && newState.VoiceChannel != null)
            {
                XElement inVoiceChannelElem = doc.Root.XPathSelectElement($"./server[@id='{newState.VoiceChannel.Guild.Id}']/InVoiceChannel");
                if (inVoiceChannelElem.Value == "")
                    return;

                OverwritePermissions permissions = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);

                var textChannel = newState.VoiceChannel.Guild.GetTextChannel(ulong.Parse(inVoiceChannelElem.Value));
                await textChannel.AddPermissionOverwriteAsync(user, permissions);
            }
            else if(oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                XElement inVoiceChannelElem = doc.Root.XPathSelectElement($"./server[@id='{oldState.VoiceChannel.Guild.Id}']/InVoiceChannel");
                if (inVoiceChannelElem.Value == "")
                    return;

                var textChannel = oldState.VoiceChannel.Guild.GetTextChannel(ulong.Parse(inVoiceChannelElem.Value));
                await textChannel.RemovePermissionOverwriteAsync(user);
            }
        }
    }
}
