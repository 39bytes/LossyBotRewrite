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
        [Summary("Toggles whether or not custom colors are enabled for the server.")]
        public async Task ToggleCustomColor(bool toggle)
        {
            XElement colorElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/CustomColor");

            colorElem.SetValue(toggle);
            await ReplyAsync($"Custom color set to `{toggle}`");
            doc.Save(Globals.path + "Servers.xml");
        }

        [Command("set InVoiceChannel")]
        [Summary("Sets the id of the in voice channel, a text channel for people who are currently in voice chat")]
        public async Task SetInVoiceChannel(ulong id)
        {
            if (Context.Guild.GetTextChannel(id) == null)
            {
                await ReplyAsync("Invalid channel!");
                return;
            }

            XElement roleElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/InVoiceChannel");
            roleElem.SetValue(id);
            await ReplyAsync($"In voice channel set to `{id}`");
            doc.Save(Globals.path + "Servers.xml");
        }
    }
}
