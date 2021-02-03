using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.Linq;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using Discord;

namespace LossyBotRewrite
{
    [Name("Image")]
    [Group("image")]
    public class ImageModule : ModuleBase<SocketCommandContext>
    {
        private readonly ImageService _imageService;
        public ImageModule(ImageService service)
        {
            _imageService = service;
        }

        [Command]
        [Summary("Apply effects to images/gifs.\n" + 
                "Valid effects: text \"[top text]\" \"[bottom text]\", magik, edge, wave, deepfry, jpgify, waaw, haah, contrast, negate, bulge, implode, drift, expand, explode, dance, angry, spectrum, lsd")]
        public async Task ImageCommand(params string[] args)
        {
            string url = args.Last();
            if (url.Contains("http"))
            {
                Array.Resize(ref args, args.Length - 1); //remove the last element
            }
            else if (Context.Message.Attachments.Count == 1)
            {
                url = Context.Message.Attachments.First().Url;
            }
            else
            {
                var messages = await Context.Channel.GetMessagesAsync(20).FlattenAsync();
                var lastAttachments = messages.Where(x => x.Attachments.Count == 1);
                if(!lastAttachments.Any())
                {
                    await ReplyAsync("`No images found in the last 20 messages.`");
                    return;
                }
                url = lastAttachments.First().Attachments.First().Url;
            }

            using (var typing = Context.Channel.EnterTypingState())
            {
                IImageWrapper? img = null;
                try
                {
                    img = await _imageService.ProcessImageAsync(url, args);
                }
                catch (Exception e)
                {
                    await ReplyAsync(e.Message);
                    if (img != null)
                    {
                        img.Dispose();
                    }
                    return;
                }

                using (Stream stream = await _imageService.WriteToStream(img))
                {
                    await Context.Channel.SendFileAsync(stream, "lossyimage." + img.GetFormat().ToString().ToLower());
                }
            }
        }
    }
}
