using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace LossyBotRewrite
{
    public class ProfilesModule : ModuleBase<SocketCommandContext>
    {
        private readonly string[] elementNames = { "fc", "sz", "tc", "rm", "cb", "main", "other" };
        
        private void CheckProfile(string userId)
        {
            XDocument doc = XDocument.Load(Globals.path + "profiles.xml");
            
            var elements = from p in doc.Root.Descendants("profile") where (p.Attribute("id").Value == userId) select p;
            if(elements.Any()) return;
            
            //Make a new profile
            XElement userElem = new XElement("profile", new XAttribute("id", userId), from el in elementNames select new XElement(el, "N/A")); //Last part creates new XElements for every element in elementNames
            doc.Root.Add();
            
            doc.Save(Globals.path + "profiles.xml");
        }
        
        private EmbedBuilder GetProfile(SocketUser user)
        {
            EmbedBuilder builder = new EmbedBuilder();
            XDocument doc = XDocument.Load(Globals.path + "profiles.xml");
            
            //straightforward
            builder.WithAvatarUrl(user.GetAvatarUrl());
            builder.WithColor(Color.Magenta);
            
            string master = "";
            foreach (string i in elementNames)
            {
                master += $"**{i.ToUpper()}**: "; //add the element name to the master string
                master += doc.Root.Elements("profile").Where(x => x.Attribute("id") == user.Id.ToString()).First().Descendants(i).Value;
                master += "\n"; //add a new line
            }
        }
        
    }
}
