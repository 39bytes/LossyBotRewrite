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

namespace LossyBotRewrite
{
    [Group("image")]
    public class ImageModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task ImageCommand(params string[] args)
        {
            ImageEffects effects = new ImageEffects();
            string url = args.Last();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            byte[] data = await DownloadImage(url);

            MagickImage img = effects.Magik(data);
            using (var stream = new MemoryStream())
            {
                img.Write(stream, MagickFormat.Jpg);
                stream.Position = 0;
                await Context.Channel.SendFileAsync(stream, "image.jpg");
            }
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            //img.Write("image.png");
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);
            return data;
        }
    }

    public class ImageEffects
    {
        public MagickImage Magik(byte[] data)
        {
            MagickImage image = new MagickImage(data);
            image.LiquidRescale(new Percentage(65), new Percentage(65));
            image.Resize(new Percentage(135), new Percentage(135));
            return image;
        }
    }
}
