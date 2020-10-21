using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    [Name("Random")]
    [Summary("For generating things randomly")]
    public class RandomModule : ModuleBase<SocketCommandContext>
    {
        [Command("rtd")]
        [Summary("Generate a random number between 1 and the input number")]
        public async Task Rtd(int sides)
        {
            if (sides > 1)
            {
                Random rnd = new Random();
                int value = rnd.Next(1, sides + 1);
                await ReplyAsync(Context.User.Mention + " :game_die: " + sides + " sided die rolled a " + value);
            }
            else
            {
                await ReplyAsync(Context.User.Mention + " Die must be more than 1 side");
            }
        }

        [Command("choose")]
        [Summary("Chooses a random option from the given options")]
        public async Task Choose([Remainder] string input)
        {
            string[] choices = input.Split(' ');
            Random rand = new Random();
            await ReplyAsync(Context.User.Mention + ":game_die: I choose: " + choices[rand.Next(choices.Length)]);
        }

        [Command("random")]
        [Summary("Generates a random weapon, map, mode, private battle game, or user. random [option]")]
        public async Task Random(string input)
        {
            Random rand = new Random();
            string[] list;
            switch (input)
            {
                case "weapon":
                    await ReplyAsync(":gun: " + Context.User.Mention, false, GetWeaponElement(rand.Next(weapons.Length)).Build());
                    return;
                case "map":
                    list = maps;
                    await ReplyAsync(":map: " + Context.User.Mention + " " + list[rand.Next(list.Count())]);
                    return;
                case "mode":
                    list = modes;
                    await ReplyAsync(":gear: " + Context.User.Mention + " " + list[rand.Next(list.Count())]);
                    return;
                case "pb":
                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle("Random Private Battle Game");

                    list = maps;
                    builder.AddField("Map :", list[rand.Next(list.Count())]);

                    list = modes;
                    builder.AddField("Mode :", list[rand.Next(list.Count())]);

                    await ReplyAsync("", false, builder.Build());
                    return;
                case "user":
                    await ReplyAsync(Context.User.Mention + " I select: " + Context.Guild.Users.ElementAt(rand.Next(Context.Guild.Users.Count)).Username); 
                    return; 
            }
            await ReplyAsync("Invalid input");
        }

        
        private EmbedBuilder GetWeaponElement(int index)
        {
            EmbedBuilder builder = new EmbedBuilder();
            string[] items = { "Weapon", "Sub", "Special", "Cost", "Level", "Class" };
            string master = "";
            for (int i = 1; i < items.Length; i++)
            {
                master += "**" + items[i] + "**: " + weapons[index][i] + "\n";
            }
            builder.AddField(weapons[index][0], master);
            return builder;
        }

        private string[] maps =
        {
            "The Reef",
            "Musselforge Fitness",
            "Starfish Mainstage",
            "Humpback Pump Track",
            "Inkblot Art Academy",
            "Moray Towers",
            "Port Mackerel",
            "Sturgeon Shipyard",
            "Manta Maria",
            "Kelp Dome",
            "Snapper Canal",
            "Blackbelly Skatepark",
            "MakoMart",
            "Walleye Warehouse",
            "Shellendorf Institute",
            "Arowana Mall",
            "Goby Arena",
            "Piranha Pit",
            "Camp Triggerfish",
            "Wahoo World",
            "New Albacore Hotel",
            "Ancho-V Games",
            "Skipper Pavilion"
        };

        private string[] modes =
        {
            "Splat Zones",
            "Tower Control",
            "Rainmaker",
            "Clam Blitz"
        };

        private static string[][] weapons = new string[][]
        {
            new string[] { ".52 Gal", "Point Sensor", "Baller", "9500", "14", "Shooter"},
            new string[] { ".52 Gal Deco", "Curling Bomb", "Sting Ray", "12700", "22", "Shooter"},
            new string[] { ".96 Gal", "Sprinkler", "Ink Armor", "12600", "21", "Shooter"},
            new string[] { ".96 Gal Deco", "Splash Wall", "Splashdown", "16200", "26", "Shooter"},
            new string[] { "Aerospray MG", "Suction Bomb", "Curling Bomb Launcher", "4900", "6", "Shooter"},
            new string[] { "Aerospray RG", "Sprinkler", "Baller", "16900", "28", "Shooter"},
            new string[] { "Ballpoint Splatling", "Toxic Mist", "Inkjet", "11600", "25", "Splatling"},
            new string[] { "Ballpoint Splatling Nouveau", "Squid Beakon", "Ink Storm", "15800", "28", "Splatling"},
            new string[] { "Bamboozler 14 Mk I", "Curling Bomb", "Tenta Missiles", "9500", "18", "Charger"},
            new string[] { "Bamboozler 14 Mk II", "Toxic Mist", "Burst Bomb Launcher", "10500", "21", "Charger"},
            new string[] { "Blaster", "Toxic Mist", "Splashdown", "3000", "5", "Shooter"},
            new string[] { "Bloblobber", "Splash Wall", "Ink Storm", "10000", "11", "Slosher"},
            new string[] { "Bloblobber Deco", "Sprinkler", "Suction Bomb Launcher", "17600", "20", "Slosher"},
            new string[] { "Carbon Roller", "Autobomb", "Ink Storm", "6200", "7", "Roller"},
            new string[] { "Carbon Roller Deco", "Burst Bomb", "Autobomb Launcher", "8500", "10", "Roller"},
            new string[] { "Clash Blaster", "Splat Bomb", "Sting Ray", "18200", "30", "Shooter"},
            new string[] { "Clash Blaster Neo", "Curling Bomb", "Tenta Missiles", "20500", "30", "Shooter"},
            new string[] { "Classic Squiffer", "Point Sensor", "Ink Armor", "8900", "12", "Charger"},
            new string[] { "Custom Blaster", "Autobomb", "Inkjet", "15300", "27", "Shooter"},
            new string[] { "Custom Dualie Squelchers", "Splat Bomb", "Ink Storm", "12900", "28", "Dualie"},
            new string[] { "Custom E-liter 4k", "Squid Beakon", "Bubble Blower", "14700", "26", "Charger"},
            new string[] { "Custom E-liter 4k Scope", "Squid Beakon", "Bubble Blower", "29900", "30", "Charger"},
            new string[] { "Custom Explosher", "Point Sensor", "Baller", "12400", "22", "Slosher"},
            new string[] { "Custom Goo Tuber", "Curling Bomb", "Inkjet", "19300", "28", "Charger"},
            new string[] { "Custom Hydra Splatling", "Ink Mine", "Ink Armor", "33300", "29", "Splatling"},
            new string[] { "Custom Jet Squelcher", "Burst Bomb", "Sting Ray", "15900", "27", "Shooter"},
            new string[] { "Custom Range Blaster", "Curling Bomb", "Bubble Blower", "11400", "18", "Shooter"},
            new string[] { "Custom Splattershot Jr.", "Autobomb", "Ink Storm", "1900", "4", "Shooter"},
            new string[] { "Dapple Dualies", "Squid Beakon", "Suction Bomb Launcher", "14700", "26", "Dualie"},
            new string[] { "Dapple Dualies Nouveau", "Toxic Mist", "Ink Storm", "17500", "29", "Dualie"},
            new string[] { "Dark Tetra Dualies", "Autobomb", "Splashdown", "10700", "14", "Dualie"},
            new string[] { "Dualie Squelchers", "Point Sensor", "Tenta Missiles", "9800", "12", "Dualie"},
            new string[] { "Dynamo Roller", "Ink Mine", "Sting Ray", "24500", "20", "Roller"},
            new string[] { "E-liter 4k", "Ink Mine", "Ink Storm", "13900", "20", "Charger"},
            new string[] { "E-liter 4k Scope", "Ink Mine", "Ink Storm", "23200", "30", "Charger"},
            new string[] { "Enperry Splat Dualies", "Curling Bomb", "Inkjet", "9000", "11", "Dualie"},
            new string[] { "Explosher", "Sprinkler", "Bubble Blower", "10800", "18", "Slosher"},
            new string[] { "Firefin Splat Charger", "Splash Wall", "Suction Bomb Launcher", "10600", "16", "Charger"},
            new string[] { "Firefin Splatterscope", "Splash Wall", "Suction Bomb Launcher", "13900", "25", "Charger"},
            new string[] { "Flingza Roller", "Splash Wall", "Splat Bomb Launcher", "15700", "24", "Roller"},
            new string[] { "Foil Squeezer", "Splat Bomb", "Bubble Blower", "10900", "25", "Shooter"},
            new string[] { "Forge Splattershot Pro", "Suction Bomb", "Bubble Blower", "20800", "20", "Shooter"},
            new string[] { "Glooga Dualies", "Ink Mine", "Inkjet", "11800", "17", "Dualie"},
            new string[] { "Glooga Dualies Deco", "Splash Wall", "Baller", "13700", "24", "Dualie"},
            new string[] { "Gold Dynamo Roller", "Splat Bomb", "Ink Armor", "29000", "25", "Roller"},
            new string[] { "Goo Tuber", "Suction Bomb", "Splashdown", "13400", "22", "Charger"},
            new string[] { "H-3 Nozzlenose", "Point Sensor", "Tenta Missiles", "17200", "29", "Shooter"},
            new string[] { "H-3 Nozzlenose D", "Suction Bomb", "Ink Armor", "18400", "30", "Shooter"},
            new string[] { "Heavy Splatling", "Sprinkler", "Sting Ray", "7800", "8", "Splatling"},
            new string[] { "Heavy Splatling Deco", "Splash Wall", "Bubble Blower", "9600", "12", "Splatling"},
            new string[] { "Hydra Splatling", "Autobomb", "Splashdown", "18500", "27", "Splatling"},
            new string[] { "Inkbrush", "Splat Bomb", "Splashdown", "2600", "5", "Roller"},
            new string[] { "Inkbrush Nouveau", "Ink Mine", "Baller", "7000", "7", "Roller"},
            new string[] { "Jet Squelcher", "Toxic Mist", "Tenta Missiles", "11300", "17", "Shooter"},
            new string[] { "Kensa .52 Gal", "Splash Wall", "Booyah Bomb", "15600", "25", "Shooter"},
            new string[] { "Kensa Charger", "Sprinkler", "Baller", "14500", "19", "Charger"},
            new string[] { "Kensa Dynamo Roller", "Sprinkler", "Booyah Bomb", "32300", "29", "Roller"},
            new string[] { "Kensa Glooga Dualies", "Fizzy Bomb", "Ink Armor", "17000", "27", "Dualie"},
            new string[] { "Kensa L-3 Nozzlenose", "Splash Wall", "Ultra Stamp", "17700", "27", "Shooter"},
            new string[] { "Kensa Luna Blaster", "Fizzy Bomb", "Ink Storm", "16600", "26", "Shooter"},
            new string[] { "Kensa Mini Splatling", "Toxic Mist", "Ultra Stamp", "18800", "29", "Splatling"},
            new string[] { "Kensa Octobrush", "Suction Bomb", "Ultra Stamp", "13200", "20", "Roller"},
            new string[] { "Kensa Rapid Blaster", "Torpedo", "Baller", "13500", "21", "Shooter"},
            new string[] { "Kensa Sloshing Machine", "Fizzy Bomb", "Splashdown", "20700", "21", "Slosher"},
            new string[] { "Kensa Splat Dualies", "Suction Bomb", "Baller", "13000", "16", "Dualie"},
            new string[] { "Kensa Splat Roller", "Splat Bomb", "Bubble Blower", "12300", "14", "Roller"},
            new string[] { "Kensa Splatterscope", "Sprinkler", "Baller", "20400", "28", "Charger"},
            new string[] { "Kensa Splattershot", "Suction Bomb", "Tenta Missiles", "5300", "6", "Shooter"},
            new string[] { "Kensa Splattshot Jr.", "Torpedo", "Bubble Blower", "8700", "9", "Shooter"},
            new string[] { "Kensa Splattshot Pro", "Splat Bomb", "Booyah Bomb", "21900", "23", "Shooter"},
            new string[] { "Kensa Undercover Brella", "Torpedo", "Ink Armor", "14800", "24", "Brella"},
            new string[] { "Krak-On Splat Roller", "Squid Beakon", "Baller", "9200", "12", "Roller"},
            new string[] { "L-3 Nozzlenose", "Curling Bomb", "Baller", "10400", "18", "Shooter"},
            new string[] { "L-3 Nozzlenose D", "Burst Bomb", "Inkjet", "12500", "23", "Shooter"},
            new string[] { "Light Tetra Dualies", "Sprinker", "Autobomb Launcher", "13300", "21", "Dualie"},
            new string[] { "Lina Blaster", "Splat Bomb", "Baller", "12100", "19", "Shooter"},
            new string[] { "Luna Blaster Neo", "Ink Mine", "Suction Bomb Launcher", "16600", "24", "Shooter"},
            new string[] { "Mini Splatling", "Burst Bomb", "Tenta Missiles", "12300", "23", "Splatling"},
            new string[] { "N-ZAP '85", "Suction Bomb", "Ink Armor", "7100", "9", "Shooter"},
            new string[] { "N-ZAP '89", "Autobomb", "Tenta Missiles", "8800", "11", "Shooter"},
            new string[] { "Nautilus 47", "Point Sensor", "Baller", "14700", "26", "Splatling"},
            new string[] { "Nautilus 79", "Suction Bomb", "Inkjet", "27900", "30", "Splatling"},
            new string[] { "Neo Splash-o-matic", "Burst Bomb", "Suction Bomb Launcher", "16800", "27", "Shooter"},
            new string[] { "Neo Sploosh-o-matic", "Squid Beakon", "Tenta Missiles", "12200", "18", "Shooter"},
            new string[] { "New Squiffer", "Autobomb", "Baller", "11000", "17", "Charger"},
            new string[] { "Octobrush", "Autobomb", "Inkjet", "8200", "10", "Roller"},
            new string[] { "Octobrush Nouveau", "Squid Beakon", "Tenta Missiles", "9900", "15", "Roller"},
            new string[] { "Range Blaster", "Suction Bomb", "Ink Storm", "9300", "14", "Shooter"},
            new string[] { "Rapid Blaster", "Ink Mine", "Splat Bomb Launcher", "9800", "13", "Shooter"},
            new string[] { "Rapid Blaster Deco", "Suction Bomb", "Inkjet", "11500", "16", "Shooter"},
            new string[] { "Rapid Blaster Pro", "Toxic Mist", "Ink Storm", "12800", "22", "Shooter"},
            new string[] { "Rapid Blaster Pro Deco", "Splash Wall", "Ink Armor", "14000", "24", "Shooter"},
            new string[] { "Slosher", "Suction Bomb", "Tenta Missiles", "2500", "5", "Slosher"},
            new string[] { "Slosher Deco", "Prinkler", "Baller", "8000", "8", "Slosher"},
            new string[] { "Sloshing Machine", "Autobomb", "Sting Ray", "12600", "13", "Slosher"},
            new string[] { "Sloshing Machine Neo", "Point Sensor", "Splat Bomb Launcher", "19800", "19", "Slosher"},
            new string[] { "Sorella Brella", "Autobomb", "Splat Bomb Launcher", "12000", "15", "Brella"},
            new string[] { "Splash-o-matic", "Toxic Mist", "Inkjet", "11200", "25", "Shooter"},
            new string[] { "Splat Brella", "Sprinkler", "Ink Storm", "8300", "9", "Brella"},
            new string[] { "Splat Charger", "Splat Bomb", "Sting Ray", "2200", "3", "Charger"},
            new string[] { "Splat Dualies", "Burst Bomb", "Tenta Missiles", "2400", "4", "Dualie"},
            new string[] { "Splat Roller", "Curling Bomb", "Splashdown", "1800", "3", "Roller"},
            new string[] { "Splatterscope", "Splat Bomb", "Sting Ray", "11400", "15", "Charger"},
            new string[] { "Splattershot", "Burst Bomb", "Splashdown", "900", "2", "Shooter"},
            new string[] { "Splattershot Jr.", "Splat Bomb", "Ink Armor", "0", "1", "Shooter"},
            new string[] { "Splattershot Pro", "Point Sensor", "Ink Storm", "13800", "10", "Shooter"},
            new string[] { "Sploosh-o-matic", "Curling Bomb", "Splashdown", "9700", "10", "Shooter"},
            new string[] { "Squeezer", "Splash Wall", "Sting Ray", "9400", "16", "Shooter"},
            new string[] { "Tenta Brella", "Squid Beakon", "Bubble Blower", "14200", "23", "Brella"},
            new string[] { "Tenta Sorella Brella", "Splash Wall", "Curling Bomb Launcher", "18600", "28", "Brella"},
            new string[] { "Tentatek Splattershot", "Splat Bomb", "Inkjet", "2100", "4", "Shooter"},
            new string[] { "Tri-Slosher", "Burst Bomb", "Ink Armor", "10200", "15", "Slosher"},
            new string[] { "Tri-Slosher Nouveau", "Splat Bomb", "Ink Storm", "11700", "17", "Slosher"},
            new string[] { "Undercover Brella", "Ink Mine", "Splashdown", "9100", "13", "Brella"},
            new string[] { "Undercover Sorella Brella", "Splat Bomb", "Baller", "11900", "19", "Brella"},
            new string[] { "Zink Mini Splatling", "Curling Bomb", "Ink Storm", "15400", "26", "Splatling"},
            new string[] { "Aerospray PG", "Burst Bomb", "Booyah Bomb", "19000", "29", "Shooter"},
            new string[] { "Bamboozler 14 Mk III", "Fizzy Bomb", "Bubble Blower", "14400", "27", "Charger"},
            new string[] { "Clear Dapple Dualies", "Torpedo", "Splashdown", "22300", "30", "Dualie"},
            new string[] { "Cherry H-3 Nozzlenose", "Splash Wall", "Bubble Blower", "26600", "30", "Shooter"},
            new string[] { "Fresh Squiffer", "Suction Bomb", "Inkjet", "14100", "24", "Charger"},
            new string[] { "Grim Range Blaster", "Burst Bomb", "Tenta Missiles", "14900", "23", "Shooter"},
            new string[] { "Heavy Splatling Remix", "Point Sensor", "Booyah Bomb", "18300", "19", "Splatling"},
            new string[] { "N-ZAP '83", "Sprinkler", "Ink Storm", "11100", "19", "Shooter"},
            new string[] { "Permanent Inkbrush", "Sprinkler", "Ink Armor", "8400", "12", "Roller"},
            new string[] { "Soda Slosher", "Splat Bomb", "Burst Bomb Launcher", "13100", "16", "Slosher"},
            new string[] { "Sploosh-o-matic 7", "Splat Bomb", "Ultra Stamp", "14600", "23", "Shooter"},
            new string[] { "Tenta Camo Brella", "Ink Mine", "Ultra Stamp", "29800", "30", "Brella"},
        };
    }
}
