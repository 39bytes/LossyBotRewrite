using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

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
        }
    }
}
