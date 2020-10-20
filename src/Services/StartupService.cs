using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Tweetinvi;

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

        private bool firstStartup = false;

        public async Task StartAsync()
        {
            string token = _config["tokens:discord"];
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Enter bot token into config.yml in the root directory.");

            SetUpFilesystem();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            TwitterAuthenticate();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider); // Load commands and modules into the command service
        }

        private void SetUpServersXML()
        {
            Console.WriteLine("Created Servers.xml");

            string serversPath = $"{Globals.path}Servers.xml";

            XDocument xml = XDocument.Load(serversPath);

            //Get the ids of every server already written in Servers.xml
            var ids = (from element in xml.Root.Descendants("server") select ulong.Parse(element.Attribute("id").Value)).ToList(); 

            foreach (SocketGuild guild in _client.Guilds)
            {
                if (!ids.Contains(guild.Id))
                {
                    xml.Root.Add(new XElement("server", 
                                 new XAttribute("id", guild.Id),
                                 new XElement("CustomColor", false),
                                 new XElement("InVoiceChannel", null)));
                }
            }

            xml.Save(serversPath);
        }

        private void SetUpFilesystem()
        {
            Console.WriteLine("Setting up filesystem...");
            if (!Directory.Exists(Globals.path))
                Directory.CreateDirectory(Globals.path);

            List<string> filenames = new List<string>()
            {
                "profiles.xml",
                "tags.xml"
            };

            foreach (string file in filenames)
            {
                if (!File.Exists(Globals.path + file))
                {
                    CreateEmptyXML(file);
                    Console.WriteLine("Created " + file);
                }
            }

            if(!File.Exists(Globals.path + "Servers.xml"))
            {
                CreateEmptyXML("Servers.xml");
                firstStartup = true;
            }
        }

        private async Task OnReady()
        {
            if (firstStartup)
            {
                SetUpServersXML();
                firstStartup = false;
            }
            await Task.CompletedTask;
        }

        private void CreateEmptyXML(string name)
        {
            using (StreamWriter sw = File.CreateText($"{Globals.path}{name}"))
            {
                sw.WriteLine("<root></root>");
            }
        }

        private void TwitterAuthenticate()
        {
            Auth.SetUserCredentials(_config["tokens:twitter:api"], _config["tokens:twitter:apiSecret"], _config["tokens:twitter:token"], _config["tokens:twitter:tokenSecret"]);
        }
    }
}
