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
    [Name("Admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        XDocument doc = XDocument.Load(Globals.path + "Servers.xml");
        [Command("toggle color")]
        [Summary("Toggles whether or not custom colors are enabled for the server.\n`toggle color [true/false]`")]
        public async Task ToggleCustomColor(bool toggle)
        {
            XElement colorElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/CustomColor");

            colorElem.SetValue(toggle);
            await ReplyAsync($"Custom color set to `{toggle}`");
            doc.Save(Globals.path + "Servers.xml");
        }

        [Command("set")]
        [Summary("Sets the id of a special channel.\n`set [InVoiceChannel/PrivateChannel] [channel id]`")]
        public async Task SetChannel(string channelType, ulong id)
        {
            if(channelType != "InVoiceChannel" && channelType != "PrivateChannel")
            {
                await ReplyAsync("Invalid channel type!");
                return;
            }
            if (Context.Guild.GetTextChannel(id) == null)
            {
                await ReplyAsync("Invalid channel!");
                return;
            }

            XElement roleElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/{channelType}");
            roleElem.SetValue(id);
            await ReplyAsync($"{channelType} set to `{id}`");
            doc.Save(Globals.path + "Servers.xml");
        }
    }
}
