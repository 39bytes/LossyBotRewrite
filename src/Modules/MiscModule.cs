using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;
using System.Runtime.InteropServices;

namespace LossyBotRewrite
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("pfp")]
        public async Task PfpCommand()
        {
            await ReplyAsync(Context.User.GetAvatarUrl(Discord.ImageFormat.Auto, 512));
        }

        [Command("pfp")]
        public async Task PfpCommand(SocketGuildUser user)
        {
            await ReplyAsync(user.GetAvatarUrl(Discord.ImageFormat.Auto, 512));
        }

        [Command("kill")]
        public async Task KillCommand(SocketGuildUser user)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(user.GetAvatarUrl(Discord.ImageFormat.Auto, 512));
            using(MagickImage pfp = new MagickImage(data))
            {
                using (MagickImage kill = new MagickImage(Globals.path + "kill.png"))
                {
                    kill.Resize(pfp.Width, pfp.Height);
                    pfp.Composite(kill, CompositeOperator.Over);
                }
                using(var stream = new MemoryStream())
                {
                    pfp.Write(stream);
                    stream.Position = 0;
                    await Context.Channel.SendFileAsync(stream, "killed.png");
                }
            }
        }
    }
}
