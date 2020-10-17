using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace LossyBotRewrite
{
    public class VoiceService
    {
        private readonly DiscordSocketClient _client;
        private IVoiceChannel voiceChannel;

        private Queue<string> queue = new Queue<string>();
        private ulong id;
        public int FFmpegId { get; private set; }

        public VoiceService(DiscordSocketClient client, IVoiceChannel channel, ulong guildId)
        {
            _client = client;
            id = guildId;
            voiceChannel = channel;
        }


        public async Task PlayAudioAsync()
        {
            var audioClient = await voiceChannel.ConnectAsync(selfDeaf: true);
            while (queue.Count != 0)
            {
                await DownloadVideo(queue.Dequeue()); //dequeue the latest video
                using (var ffmpeg = CreateStream($"{id}.mp3"))
                using (var output = ffmpeg.StandardOutput.BaseStream)
                using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    FFmpegId = ffmpeg.Id;
                    try
                    {
                        await output.CopyToAsync(discord);
                    }
                    finally
                    {
                        await discord.FlushAsync();
                    }
                }
            }
            await audioClient.StopAsync();
        }

        private async Task DownloadVideo(string videoId)
        {
            YoutubeClient youtube = new YoutubeClient();
            var manifest = await youtube.Videos.Streams.GetManifestAsync(videoId);

            var streamInfo = manifest.GetAudioOnly().WithHighestBitrate();

            if(streamInfo != null)
            {
                var stream = youtube.Videos.Streams.GetAsync(streamInfo);

                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{id}.mp3");
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
        public void AddToQueue(string videoId)
        {
            queue.Enqueue(videoId);
        }
    }
}
