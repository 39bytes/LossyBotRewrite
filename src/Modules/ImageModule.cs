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
using AngleSharp.Html.Dom;

namespace LossyBotRewrite
{
    [Name("Image")]
    [Group("image")]
    public class ImageModule : ModuleBase<SocketCommandContext>
    {
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
            var typing = Context.Channel.EnterTypingState();
            try
            {
                IImageWrapper? img = await ProcessImageAsync(url, args);
                if(img == null)
                {
                    return;
                }

                using (var stream = new MemoryStream())
                {
                    img.Write(stream);
                    var im = new MagickImage();
                    stream.Position = 0;
                    await Context.Channel.SendFileAsync(stream, "lossyimage." + img.GetFormat().ToString().ToLower());
                }
            }
            finally
            {
                typing.Dispose();
            }
        }

        private async Task<byte[]> DownloadImageAsync(string url)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);
            return data;
        }

        private async Task<IImageWrapper?> ProcessImageAsync(string url, string[] args)
        {
            IImageWrapper? img;

            try
            {
                if (url.Contains(".gif"))
                    img = new GifWrapper(await DownloadImageAsync(url));
                else
                    img = new ImageWrapper(await DownloadImageAsync(url));
            }
            catch (Exception)
            {
                await ReplyAsync("Invalid url!");
                return null;
            }
            

            if (args[0].ToLower() == "text")
            {
                string top = "";
                string bottom = "";
                if(args.Length > 1)
                    top = args[1];
                if (args.Length > 2)
                    bottom = args[2];
                img.Text(top, bottom);
                return img;
            }

            foreach (var effect in args)
            {
                switch (effect.ToLower())
                {
                    case "magik":
                        img.Magik();
                        break;
                    case "edge":
                        img.Edge();
                        break;
                    case "wave":
                        img.Wave();
                        break;
                    case "deepfry":
                        img.Deepfry();
                        break;
                    case "jpgify":
                        img.Jpgify();
                        break;
                    case "waaw":
                        img.Waaw();
                        break;
                    case "haah":
                        img.Haah();
                        break;
                    case "contrast":
                        img.Contrast();
                        break;
                    case "negate":
                        img.Negate();
                        break;
                    case "bulge":
                        img.Bulge();
                        break;
                    case "implode":
                        img = img.Implode();
                        break;
                    case "drift":
                        img = img.Drift();
                        break;
                    case "expand":
                        img = img.Expand();
                        break;
                    case "explode":
                        img = img.Explode();
                        break;
                    case "dance":
                        img = img.Dance();
                        break;
                    case "angry":
                        img = img.Angry();
                        break;
                    case "spectrum":
                        img = img.Spectrum();
                        break;
                    case "lsd":
                        img = img.Lsd();
                        break;
                    default:
                        await ReplyAsync($"`Invalid effect '{effect}'.`");
                        img.Dispose();
                        throw new Exception("Invalid effect");
                }
            }
            return img;
        }
    }
}
