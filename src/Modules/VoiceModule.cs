using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace LossyBotRewrite
{
    [Group("voice")]
    public class VoiceModule : ModuleBase<SocketCommandContext>
    {
        private readonly VoiceServiceManager _voiceManager;

        public VoiceModule(VoiceServiceManager manager)
        {
            _voiceManager = manager;
        }

        [Command("play")]
        public async Task VoicePlay(string url)
        {
            if((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }

            var client = new YoutubeClient();
            var info = await client.Videos.GetAsync(url);
            await ReplyAsync("", false, GetVideoEmbed(info).Build());

            if (_voiceManager.HasActiveService(Context.Guild.Id))
            {
                _voiceManager.AddVideoToServiceQueue(Context.Guild.Id, info);
            }
            else
            {
                await _voiceManager.CreateVoiceService((Context.User as IGuildUser).VoiceChannel, info);
            }
        }

        [Command("skip")]
        public async Task SkipSong()
        {
            if ((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }
            _voiceManager.KillFFMpegProcess(Context.Guild.Id);
            await ReplyAsync(":fast_forward: Skipped song");
        }

        [Command("queue")]
        public async Task VoiceQueue()
        {
            if ((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }
            Video playing = _voiceManager.GetCurrentlyPlaying(Context.Guild.Id);
            TimeSpan? uptime = _voiceManager.GetFFMpegUptime(Context.Guild.Id);

            if(uptime == null)
            {
                await ReplyAsync("No song currently playing!");
                return;
            }

            int playPercent = (int)(100 * uptime.Value.TotalSeconds / playing.Duration.TotalSeconds);
            string progressBar = new string('#', playPercent / 5) + new string('.', 20 - (playPercent / 5));

            string progress = uptime.Value.ToString("m\\:ss"); 
            string duration = playing.Duration.ToString("m\\:ss"); 

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(":musical_note: Queue");
            builder.WithThumbnailUrl(playing.Thumbnails.MediumResUrl);
            builder.AddField("Now Playing", $"**{playing.Title}**\n" +
                                            $"`{progressBar}`\n{progress} / {duration}\n" +
                                            $"By: {playing.Author}");
                                            

            await ReplyAsync("", false, builder.Build());
        }

        private EmbedBuilder GetVideoEmbed(Video video)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(":musical_note: Added song to queue!");
            builder.AddField(video.Title, $"**By:** {video.Author}\n**Length:** {video.Duration.ToString().Substring(3)}");
            builder.WithThumbnailUrl(video.Thumbnails.MediumResUrl);
            return builder;
        }
    }
}
