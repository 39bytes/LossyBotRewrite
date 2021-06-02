using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;

namespace LossyBotRewrite
{
    [Name("Misc")]
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        private readonly ImageService _service;
        public MiscModule(ImageService service)
        {
            _service = service;
        }
        [Command("pfp")]
        public async Task PfpCommand()
        {
            await ReplyAsync(Context.User.GetAvatarUrl(Discord.ImageFormat.Auto, 512));
        }

        [Command("pfp")]
        public async Task PfpCommand(SocketGuildUser user)
        {
            await ReplyAsync(user.GetAvatarUrl(Discord.ImageFormat.Auto, 512));
        }

        [Command("kill")]
        public async Task KillCommand(SocketGuildUser user)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(user.GetAvatarUrl(Discord.ImageFormat.Auto, 512));
            using(MagickImage pfp = new MagickImage(data))
            {
                using (MagickImage kill = new MagickImage(Globals.path + "kill.png"))
                {
                    kill.Resize(pfp.Width, pfp.Height);
                    pfp.Composite(kill, CompositeOperator.Over);
                }
                using(var stream = new MemoryStream())
                {
                    pfp.Write(stream);
                    stream.Position = 0;
                    await Context.Channel.SendFileAsync(stream, "killed.png");
                }
            }
        }
        
        [Command("what")]
        public async Task WhatVideoCommand(string imageUrl = "")
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (imageUrl == "")
            {
                if (Context.Message.Attachments.Count == 1)
                {
                    imageUrl = Context.Message.Attachments.First().Url;
                }
                else
                {
                    await ReplyAsync("Include an image!");
                    return;
                }
            }

            await _service.ProcessWhatVideoAsync(imageUrl, now);
            await Context.Channel.SendFileAsync($"{now}.mp4");
            File.Delete($"{now}.jpg");
            File.Delete($"{now}.mp4");
        }

        [Command("fnaf")]
        public async Task FnafVideoCommand(string imageUrl = "")
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (imageUrl == "")
            {
                if (Context.Message.Attachments.Count == 1)
                {
                    imageUrl = Context.Message.Attachments.First().Url;
                }
                else
                {
                    await ReplyAsync("Include an image!");
                    return;
                }
            }

            await _service.AddFnafSoundAsync(imageUrl, now);
            await Context.Channel.SendFileAsync($"{now}fnaf.mp4");
            File.Delete($"{now}fnaf.png");
            File.Delete($"{now}fnaf.mp4");
        }

        [Command("reset", RunMode = RunMode.Async)]
        public async Task KillSwitch() //this is totally overdone but I think the reset command should look cool idk why
        {
            if (Context.User.Id == 201922904781357057
             || Context.User.Id == 82195381156315136
             || Context.User.Id == 244152605561847808)
            {
                int timer = 3;
                var message = await ReplyAsync($"Restarting app in {timer}.");
                do
                {
                    await Task.Delay(1000);
                    await message.ModifyAsync(x => x.Content = $"Restarting app in {timer}.");
                    timer--;
                } while (timer != 0);
                await message.ModifyAsync(x => x.Content = $"Goodbye");
                Environment.Exit(1);
            }
            else
            {
                await ReplyAsync(Context.User.Mention + " You cannot use this command.");
            }
        }
    }
}
