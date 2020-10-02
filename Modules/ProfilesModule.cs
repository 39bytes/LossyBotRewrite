using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace LossyBotRewrite
{
    public class ProfilesModule : ModuleBase<SocketCommandContext>
    {
        
        
        private void CheckProfile(string userId)
        {
            XDocument doc = XDocument.Load(Globals.path + "profiles.xml");
            
            var elements = from p in doc.Root.Descendants("profile") where (p.Attribute("id").Value == userId) select p;
            if(elements.Any()) return;
        }
    }
}
