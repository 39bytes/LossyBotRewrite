using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;

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
                _voiceManager.AddVideoToServiceQueue(Context.Guild.Id, info.Id);
            }
            else
            {
                await _voiceManager.CreateVoiceService((Context.User as IGuildUser).VoiceChannel, info);
            }
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
