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

namespace LossyBotRewrite
{
    [Group("image")]
    public class ImageModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task ImageCommand(params string[] args)
        {
            string url = args.Last();
            Array.Resize(ref args, args.Length - 1); //remove the last element

            List<MethodInfo> effects = new List<MethodInfo>();
            ImageEffects ie = new ImageEffects();
            Type type = ie.GetType();

            foreach (string effect in args)
            {
                effects.Add(type.GetMethod(effect, BindingFlags.IgnoreCase));
            }

            byte[] data = await DownloadImage(url);

            MagickImage img = new MagickImage(data);

            foreach(var method in effects)
            {
                img = (MagickImage)method.Invoke(ie, new MagickImage[] { img });
            }
            
            using (var stream = new MemoryStream())
            {
                img.Write(stream, MagickFormat.Jpg);
                stream.Position = 0;
                await Context.Channel.SendFileAsync(stream, "image.jpg");
            }
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);
            return data;
        }
    }

    public class ImageEffects
    {
        public MagickImage Magik(MagickImage image)
        {
            image.LiquidRescale(new Percentage(65), new Percentage(65));
            image.Resize(new Percentage(135), new Percentage(135));
            return image;
        }
    }
}
