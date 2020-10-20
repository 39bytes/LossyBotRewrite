using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Playlists;
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
            service.AddToQueue(video);
            Console.WriteLine($"Created voice service for {channel.GuildId}");

            await service.PlayAudioAsync().ContinueWith(t => DestroyVoiceService(channel.GuildId)); //Play audio, then destroy the object once finished
        }

        public async Task CreateVoiceService(IVoiceChannel channel, IEnumerable<Video> playlist)
        {
            var service = new VoiceService(_client, channel, channel.GuildId);
            activeVoiceServices.Add(channel.GuildId, service);
            service.AddPlaylistToQueue(playlist);
            Console.WriteLine($"Created voice service for {channel.GuildId}");

            await service.PlayAudioAsync().ContinueWith(t => DestroyVoiceService(channel.GuildId)); //Play audio, then destroy the object once finished
        }
        private void DestroyVoiceService(ulong id)
        {
            activeVoiceServices.Remove(id);
            File.Delete($"{id}.mp3");
            Console.WriteLine($"Destroyed voice service for {id}");
        }

        public void ForceStopService(ulong guildId)
        {
            activeVoiceServices[guildId].EmptyQueue();
            KillFFMpegProcess(guildId);
        }

        public void AddVideoToServiceQueue(ulong guildId, Video video)
        {
            activeVoiceServices[guildId].AddToQueue(video);
        }

        public void AddPlaylistToServiceQueue(ulong guildId, IEnumerable<Video> playlist)
        {
            activeVoiceServices[guildId].AddPlaylistToQueue(playlist);
        }

        public void KillFFMpegProcess(ulong guildId)
        {
            int id = activeVoiceServices[guildId].FFmpegId;

            using(Process process = Process.GetProcessById(id))
            {
                if(process.ProcessName == "ffmpeg")
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }
        }

        public TimeSpan? GetFFMpegUptime(ulong guildId)
        {
            int id = activeVoiceServices[guildId].FFmpegId;
            TimeSpan? uptime = null;
            using (Process process = Process.GetProcessById(id))
            {
                if(process.ProcessName == "ffmpeg")
                {
                    uptime = DateTime.Now - process.StartTime;
                }
            }
            return uptime;
        }

        public Video GetCurrentlyPlaying(ulong guildId)
        {
            return activeVoiceServices[guildId].CurrentlyPlaying;
        }

        public Queue<Video> GetQueue(ulong guildId)
        {
            return activeVoiceServices[guildId].Queue;
        }
    }
}
