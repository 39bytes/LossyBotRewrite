using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            if (oldState.VoiceChannel == null && newState.VoiceChannel != null)
            {
                // todo
            }
        }
    }
}
