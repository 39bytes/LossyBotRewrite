using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("pfp")]
        public async Task PfpCommand()
        {
            await ReplyAsync(Context.User.GetAvatarUrl());
        }
    }
}
