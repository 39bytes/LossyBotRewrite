using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    public class RandomModule : ModuleBase<SocketCommandContext>
    {
        [Command("rtd")]
        public async Task Rtd(int sides)
        {
            if (sides > 1)
            {
                Random rnd = new Random();
                int value = rnd.Next(1, sides + 1);
                await ReplyAsync(Context.User.Mention + " :game_die: " + sides + " sided die rolled a " + value);
            }
            else
            {
                await ReplyAsync(Context.User.Mention + " Die must be more than 1 side");
            }
        }

        [Command("choose")]
        public async Task Choose([Remainder] string input)
        {
            string[] choices = input.Split(' ');
            Random rand = new Random();
            await ReplyAsync(Context.User.Mention + ":game_die: I choose: " + choices[rand.Next(choices.Length)]);
        }
    }
}
