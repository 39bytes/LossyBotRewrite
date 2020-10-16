using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace LossyBotRewrite
{
    public class VoiceServiceManager
    {
        private readonly DiscordSocketClient _client;

        private Dictionary<ulong, VoiceService> activeVoiceServices = new Dictionary<ulong, VoiceService>();

        public VoiceServiceManager(DiscordSocketClient client)
        {
            _client = client;
        }

        public bool HasActiveService(ulong guildId)
        {
            return activeVoiceServices.ContainsKey(guildId);
        }

        public async Task CreateVoiceService(IVoiceChannel channel, Video video)
        {
            var service = new VoiceService(_client, channel, channel.GuildId);
            activeVoiceServices.Add(channel.GuildId, service);
            service.AddToQueue(video.Id);
            Console.WriteLine($"Created voice service for {channel.GuildId}");

            await service.PlayAudioAsync().ContinueWith(t => DestroyVoiceService(channel.GuildId)); //Play audio, then destroy the object once finished
        }

        private void DestroyVoiceService(ulong id)
        {
            activeVoiceServices.Remove(id);
            File.Delete($"{id}.mp3");
            Console.WriteLine($"Destroyed voice service for {id}");
        }

        public void AddVideoToServiceQueue(ulong guildId, string videoId)
        {
            activeVoiceServices[guildId].AddToQueue(videoId);
        }
    }
}
