using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LossyBotRewrite
{
    [Name("Color")]
    [Group("color")]
    public class RolesModule : ModuleBase<SocketCommandContext>
    {
        XDocument doc = XDocument.Load(Globals.path + "Servers.xml");

        private readonly List<string> colors = new List<string>
        {   "red",
            "pink",
            "purple",
            "blue",
            "teal",
            "green",
            "lime",
            "yellow",
            "orange" };

        [Command]
        public async Task ColorDefault()
        {
            await ReplyAsync(Context.User.Mention + " color (add [color]) (remove)\n:art: Spice up your user!  You can add the following colors:\n **red**, **pink**, **purple**, **blue**, **teal**, **green**, **lime**, **yellow**, **orange**");
        }

        [Command("add")]
        public async Task AddColor(string input)
        {
            if (!colors.Contains(input))
            {
                await ReplyAsync(Context.User.Mention + " Invalid color choice");
            }
            else
            {
                //Loop over each role of the user (Get the color they have)
                foreach (SocketRole role in (Context.User as SocketGuildUser).Roles)
                {
                    await (Context.User as IGuildUser).AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == input));
                    if (colors.Contains(role.Name))
                    {
                        await (Context.User as IGuildUser).RemoveRoleAsync(role);
                    }
                }
                await ReplyAsync(":art: " + Context.User.Mention + " Buh BAM! You're " + input + " now!");
            }
        }

        [Command("remove")]
        public async Task RemoveColor()
        {
            SocketRole deletThis = null;
            //Loop over each role of the user (Get the color they have)
            foreach (SocketRole role in (Context.User as SocketGuildUser).Roles)
            {
                if (colors.Contains(role.Name))
                {
                    await (Context.User as IGuildUser).RemoveRoleAsync(role);
                }
                if (role.Name == Context.User.Id.ToString())
                {
                    deletThis = role;
                }
            }
            if(deletThis != null)
            {
                await deletThis.DeleteAsync();
            }
            await ReplyAsync(Context.User.Mention + " Your color was removed and now you're super boring");
        }

        [Command("custom")]
        public async Task CustomColor(int r, int g, int b)
        {
            XElement colorElem = doc.Root.XPathSelectElement($"./server[@id='{Context.Guild.Id}']/CustomColor");

            if (colorElem.Value == "false")
            {
                await ReplyAsync("Custom colors are turned off for this server!");
                return;
            }

            if (Enumerable.Range(54 - 10, 20).Contains(r) &&
                Enumerable.Range(57 - 10, 20).Contains(g) &&
                Enumerable.Range(63 - 10, 20).Contains(b))
            {
                await ReplyAsync(Context.User.Mention + " Too close to the Discord dark theme! Choose a different color!");
                return;
            }
            if (Enumerable.Range(255 - 15, 16).Contains(r) &&
                Enumerable.Range(255 - 15, 16).Contains(g) &&
                Enumerable.Range(255 - 15, 16).Contains(b))
            {
                await ReplyAsync(Context.User.Mention + " Too close to the Discord light theme! Choose a different color!");
                return;
            }

            foreach (SocketRole role in (Context.User as SocketGuildUser).Roles)
            {
                if (colors.Contains(role.Name))
                {
                    await (Context.User as IGuildUser).RemoveRoleAsync(role);
                }
            }

            Color color = new Color(r, g, b);

            SocketRole userRole = null;
            foreach (SocketRole role in Context.Guild.Roles)
            {
                if (role.Name == Context.User.Id.ToString()) userRole = role;
            }

            if(userRole == null)
            {
                var newRole = await Context.Guild.CreateRoleAsync(Context.User.Id.ToString(), null, color, false, null);
                await (Context.User as IGuildUser).AddRoleAsync(newRole);
            }
            else
            {
                await userRole.ModifyAsync(c => c.Color = color);
                await (Context.User as IGuildUser).AddRoleAsync(userRole);
            }

            await ReplyAsync(Context.User.Mention + " You got a custom color!");
        }

    }
}
