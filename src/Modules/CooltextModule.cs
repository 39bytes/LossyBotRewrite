using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.Commands;

namespace LossyBotRewrite
{
    public class CooltextModule : ModuleBase<SocketCommandContext>
    {
        private readonly CooltextService _cooltextService;
        public CooltextModule(CooltextService service)
        {
            _cooltextService = service;
        }

        [Command("cooltext")]
        public async Task Cooltext(string text)
        {
            string filename = await _cooltextService.GetBurningTextAsync(text);
            await Context.Channel.SendFileAsync(filename);
            File.Delete(filename);
        }
    }
}
