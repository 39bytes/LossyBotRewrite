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

            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            List<MethodInfo> effects = new List<MethodInfo>();
            ImageEffects ie = new ImageEffects();
            Type type = ie.GetType();

            foreach (string effect in args)
            {
                effects.Add(type.GetMethod(effect, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance));
            }

            byte[] data = await DownloadImage(url);

            MagickImage img = new MagickImage(data);

            foreach(var method in effects)
            {
                img = (MagickImage)method.Invoke(ie, new object[] { img });
            }
            
            using (var stream = new MemoryStream())
            {
                img.Write(stream, MagickFormat.Jpg);
                stream.Position = 0;
                await Context.Channel.SendFileAsync(stream, "lossyimage.jpg");
            }

            img.Dispose();

            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);
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
        
        public MagickImage Edge(MagickImage image)
        {
            image.Edge(3);
            return image;
        }
        
        public MagickImage Wave(MagickImage image)
        {
            image.Wave();
            return image;
        }
        
        public MagickImage Deepfry(MagickImage image)
        {
            image.Resize((Percentage)50);
            image.AddNoise(NoiseType.MultiplicativeGaussian);
            image.Modulate((Percentage)100, (Percentage)300, (Percentage)100);
            image.Resize((Percentage)200);
            
            return image;
        }
        
        public MagickImage Jpgify(MagickImage image)
        {
            image.Quality = 5;
            return image;
        }

        public MagickImage Waaw(MagickImage image)
        {
            MagickImage clone = new MagickImage(image);
            clone.Flop();
            clone.Extent(clone.Width / 2, clone.Height);
            image.Composite(clone, CompositeOperator.Over);

            clone.Dispose();
            return image;
        }

        public MagickImage Haah(MagickImage image)
        {
            MagickImage clone = new MagickImage(image);
            clone.Flip();
            clone.Extent(clone.Width, clone.Height / 2);
            image.Composite(clone, CompositeOperator.Over);

            clone.Dispose();

            return image;
        }

        public MagickImage Contrast(MagickImage image)
        {
            image.BrightnessContrast((Percentage)40, (Percentage)75);

            return image;
        }

        public MagickImage Negate(MagickImage image)
        {
            image.Negate();

            return image;
        }

        public MagickImage Bulge(MagickImage image)
        {
            image.Implode(-1, PixelInterpolateMethod.Average);

            return image;
        }
    }
}
