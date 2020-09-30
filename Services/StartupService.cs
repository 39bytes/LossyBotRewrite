using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LossyBotRewrite
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient client,
            CommandService commands,
            IConfigurationRoot config)
        {
            _provider = provider;
            _config = config;
            _client = client;
            _commands = commands;

            _client.Ready += OnReady;
        }

        public async Task StartAsync()
        {
            string token = _config["tokens:discord"];
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Enter your bot's token into config.yml in the root directory.");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();


            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider); // Load commands and modules into the command service
        }

        private void SetUpFilesystem()
        {
            Console.WriteLine("Setting up filesystem...");
            if (!Directory.Exists(Globals.path))
                Directory.CreateDirectory(Globals.path);

            if (!File.Exists($"{Globals.path}{Path.DirectorySeparatorChar}Servers.xml"))
                CreateEmptyXML("Servers.xml");

            string serversPath = $"{Globals.path}{Path.DirectorySeparatorChar}Servers.xml";

            XDocument xml = XDocument.Load(serversPath);

            //Get the ids of every server already written in Servers.xml
            var ids = (from element in xml.Root.Descendants("server") select ulong.Parse(element.Attribute("id").Value)).ToList(); 

            foreach (SocketGuild guild in _client.Guilds)
            {
                if (!ids.Contains(guild.Id))
                {
                    xml.Root.Add(new XElement("server", new XAttribute("id", guild.Id)));
                }
            }

            xml.Save(serversPath);
        }

        private async Task OnReady()
        {
            SetUpFilesystem();
            await Task.CompletedTask;
        }

        private void CreateEmptyXML(string name)
        {
            using (StreamWriter sw = File.CreateText($"{Globals.path}{Path.DirectorySeparatorChar}{name}"))
            {
                sw.WriteLine("<root></root>");
            }
        }
    }
}
