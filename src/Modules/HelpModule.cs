using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }
        [Command("help")]
        public async Task HelpCommand(string name)
        {
            var builder = new EmbedBuilder();
            builder.WithColor(new Color(114, 137, 218));

            var module = _service.Modules.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            string desc = "";
            foreach(var command in module.Commands)
            {
                var result = await command.CheckPreconditionsAsync(Context);
                if (result.IsSuccess)
                    desc += $"{command.Aliases.First()}\n";
            }
        }
    }
}
