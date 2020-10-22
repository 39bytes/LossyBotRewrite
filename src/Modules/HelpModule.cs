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
            builder.WithTitle($"{module.Name} commands");
            foreach(var command in module.Commands)
            {
                if(command.Summary != null)
                {
                    builder.AddField(x =>
                    {
                        x.Name = $"{module.Group} {command.Name}";
                        x.Value = command.Summary;
                        x.IsInline = false;
                    });
                }
            }
            await ReplyAsync("", false, builder.Build());
        }
    }
}
