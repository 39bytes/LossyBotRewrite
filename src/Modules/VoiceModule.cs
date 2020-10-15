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
        }

        private EmbedBuilder GetVideoEmbed(Video video)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle("Added song to queue!");
            builder.AddField(video.Title, $"**By:** {video.Author}\n**Length:** {video.Duration.ToString().Substring(3)}");
            builder.WithThumbnailUrl(video.Thumbnails.LowResUrl);
            return builder;
        }
    }
}
