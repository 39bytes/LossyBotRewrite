using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

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

            colorElem.SetValue(toggle);
            await ReplyAsync($"Custom color set to `{toggle}`");
            doc.Save(Globals.path + "Servers.xml");
        }

        [Command("set invoice")]
        public async Task SetInVoiceRoleId(ulong id)
        {
            XElement roleElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/InVoiceRole");
            roleElem.SetValue(id);
            await ReplyAsync($"In voice role set to `{id}`");
            doc.Save(Globals.path + "Servers.xml");
        }
    }
}
