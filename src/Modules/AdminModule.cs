using Discord;
using Discord.Commands;
using Discord.Websocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LossyBotRewrite
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        XDocument doc = XDocument.Load(Globals.path + "Servers.xml");
        [Command("toggle color")]
        public async Task ToggleCustomColor(bool toggle)
        {
            XElement colorElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/CustomColor");
            
            colorElem.Value = toggle.ToString();
            await ReplyAsync($"Custom color set to `{toggle}`");
        }
    }
}
