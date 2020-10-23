using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    [Name("Maps")]
    public class SplatnetModule : ModuleBase<SocketCommandContext>
    {
        private readonly SplatnetService _splatnetService;
        
        public SplatnetModule(SplatnetService splatnet)
        {
            _splatnetService = splatnet;
        }

        [Command("maps")]
        [Summary("Gets the current maps. Defaults to ranked.\n`maps [turf/ranked/league]`)]
        public async Task GetCurrentMaps(string mode = "")
        {
            var (schedule, currentRotation) = await _splatnetService.GetScheduleAsync();

            EmbedBuilder builder = new EmbedBuilder();

            JToken modeRotations;
            switch (mode)
            {
                case "turf":
                case "regular":
                case "tw":
                    modeRotations = schedule["regular"];
                    builder.WithTitle("Regular Battle <:turf:767473472460292166>");
                    break;
                case "league":
                case "twin":
                case "quad":
                    modeRotations = schedule["league"];
                    builder.WithTitle("League Battle <:league:767473472423067698>");
                    break;
                case "ranked":
                case "rank":
                case "solo":
                case "soloq":
                default:
                    modeRotations = schedule["gachi"];
                    builder.WithTitle("Ranked Battle <:ranked:767473586851151872>");
                    break;
            }
            TimeSpan remaining = TimeSpan.FromSeconds(modeRotations[currentRotation].Value<long>("end_time") - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            builder.AddField($"Now: {modeRotations.SelectToken($"[{currentRotation}].rule.name")}", $"On **{modeRotations.SelectToken($"[{currentRotation}].stage_a.name")}** " +
                                                                                                    $"and **{modeRotations.SelectToken($"[{currentRotation}].stage_b.name")}**\n" +
                                                                                                    $"{FormatTime(remaining)} until next rotation");
            builder.WithThumbnailUrl($"https://app.splatoon2.nintendo.net{modeRotations.SelectToken($"[{currentRotation}].stage_a.image")}"); //Image of the first stage

            for (int i = 1; i < 4; i++)
            {
                remaining = remaining.Add(TimeSpan.FromSeconds(7200)); //just add 2 hours for the next rotations
                builder.AddField($"Next: {modeRotations.SelectToken($"[{currentRotation + i}].rule.name")}", $"On **{modeRotations.SelectToken($"[{currentRotation + i}].stage_a.name")}** " +
                                                                                                             $"and **{modeRotations.SelectToken($"[{currentRotation + i}].stage_b.name")}**\n" +
                                                                                                             $"In {FormatTime(remaining)}");
            }

            builder.WithFooter("All game data from splatoon2.ink");

            await ReplyAsync("", false, builder.Build());
        }

        private string FormatTime(TimeSpan time)
        {
            TimeSpan rounded = TimeSpan.FromMinutes(Math.Ceiling(time.TotalMinutes)); //round up to nearest minute
            string timeString = "";

            if(rounded.Hours > 0)
            {
                timeString += $"{rounded.Hours} hour";
                if (rounded.Hours > 1) timeString += "s";
                timeString += " and ";
            }
            if(rounded.Minutes > 0)
            {
                timeString += $"{rounded.Minutes} minute";
                if (rounded.Minutes > 1) timeString += "s";
            }
            return timeString;
        }
    }
}
