using Discord;
using Discord.Net;
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
            var oldVC = oldState.VoiceChannel;
            var newVC = newState.VoiceChannel;
            if (oldVC == null && newVC != null) //joined voice channel
            {
                ulong guild = newVC.Guild.Id;
                XElement inVoiceChannelElem = doc.Root.XPathSelectElement($"./server[@id='{guild}']/InVoiceChannel");
                if (inVoiceChannelElem.Value == "")
                    return;

                OverwritePermissions permissions = new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow);

                var textChannel = newVC.Guild.GetTextChannel(ulong.Parse(inVoiceChannelElem.Value));
                await textChannel.AddPermissionOverwriteAsync(user, permissions); //give them invoice channel perms
            }
            else if(oldVC != null && newVC == null) //left voice channel
            {
                ulong guild = oldVC.Guild.Id;
                if (_voiceManager.HasActiveService(guild))
                {
                    if (oldVC.Id == _voiceManager.GetServiceVoiceChannelId(guild))
                    {
                        if (oldVC.Users.Count == 1) //only the bot left in there
                        {
                            _voiceManager.ForceStopService(guild); //kill the service
                        }
                    }
                }
                XElement inVoiceChannelElem = doc.Root.XPathSelectElement($"./server[@id='{guild}']/InVoiceChannel");
                if (inVoiceChannelElem.Value == "")
                    return;

                var textChannel = oldVC.Guild.GetTextChannel(ulong.Parse(inVoiceChannelElem.Value));
                await textChannel.RemovePermissionOverwriteAsync(user); //remove their invoice channel perms
            }
            else if(oldVC != null && newVC != null && oldVC != newVC) //moved channel
            {
                ulong guild = oldVC.Guild.Id;
                if (_voiceManager.HasActiveService(guild))
                {
                    if (oldVC.Id == _voiceManager.GetServiceVoiceChannelId(guild))
                    {
                        if (oldVC.Users.Count == 1) //only the bot left in there
                        {
                            _voiceManager.ForceStopService(guild); //kill the service
                        }
                    }
                }
            }
        }
    }
}
