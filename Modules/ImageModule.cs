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
using YamlDotNet.Serialization.ObjectGraphVisitors;

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

            Stopwatch watch = new Stopwatch();
            watch.Start();

            byte[] data = await DownloadImage(url);

            IImageWrapper img;

            if (url.Contains(".gif"))
                img = new GifWrapper(await DownloadImage(url));
            else
                img = new ImageWrapper(await DownloadImage(url));

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
                    default:
                        await ReplyAsync($"`Invalid effect '{effect}'.`");
                        img.Dispose();
                        return;

                }
            }

            using (var stream = new MemoryStream())
            {
                img.Write(stream);
                stream.Position = 0;
                await Context.Channel.SendFileAsync(stream, "lossyimage." + img.GetFormat().ToString().ToLower());
            }

            watch.Stop();

            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);
            return data;
        }
    }

    //public class ImageEffects
    //{
    //    const int gifEffectFrameCount = 21;

    //    public void Magik(IImageWrapper image)
    //    {
    //        image.Magik();
    //    }
    //    public void Deepfry(IImageWrapper image)
    //    {
    //        image.Deepfry();
    //    }

        //public MagickImage Magik(MagickImage image)
        //{
        //    image.LiquidRescale(new Percentage(65), new Percentage(65));
        //    image.Resize(new Percentage(135), new Percentage(135));
        //    return image;
        //}
        
        //public MagickImage Edge(MagickImage image)
        //{
        //    image.Edge(3);
        //    return image;
        //}
        
        //public MagickImage Wave(MagickImage image)
        //{
        //    image.Wave();
        //    return image;
        //}
        
        //public MagickImage Deepfry(MagickImage image)
        //{
        //    image.Resize((Percentage)50);
        //    image.AddNoise(NoiseType.MultiplicativeGaussian);
        //    image.Modulate((Percentage)100, (Percentage)300, (Percentage)100);
        //    image.Resize((Percentage)200);
            
        //    return image;
        //}
        
        //public MagickImage Jpgify(MagickImage image)
        //{
        //    image.Quality = 5;
        //    return image;
        //}

        //public MagickImage Waaw(MagickImage image)
        //{
        //    MagickImage clone = new MagickImage(image);
        //    clone.Flop();
        //    clone.Extent(clone.Width / 2, clone.Height);
        //    image.Composite(clone, CompositeOperator.Over);

        //    clone.Dispose();
        //    return image;
        //}

        //public MagickImage Haah(MagickImage image)
        //{
        //    MagickImage clone = new MagickImage(image);
        //    clone.Flip();
        //    clone.Extent(clone.Width, clone.Height / 2);
        //    image.Composite(clone, CompositeOperator.Over);

        //    clone.Dispose();

        //    return image;
        //}

        //public MagickImage Contrast(MagickImage image)
        //{
        //    image.BrightnessContrast((Percentage)40, (Percentage)75);

        //    return image;
        //}

        //public MagickImage Negate(MagickImage image)
        //{
        //    image.Negate();

        //    return image;
        //}


        //public MagickImage Bulge(MagickImage image)
        //{
        //    image.Implode(-1, PixelInterpolateMethod.Average);

        //    return image;
        //}

        //public MagickImageCollection Expand(MagickImage image)
        //{
        //    double percent;
        //    MagickImage original = new MagickImage();
        //    original.CopyPixels(image);
        //    int width = image.Width;
        //    int height = image.Height;
        //    var collection = new MagickImageCollection();
            
        //    for(int i = gifEffectFrameCount - 1; i >= 0; i--)
        //    {
        //        percent = 100 - (Math.Floor((double)100 / (gifEffectFrameCount + 1)) * i);
        //        image.CopyPixels(original);
        //        image.LiquidRescale(new Percentage(percent));
        //        image.Resize(width, height);
        //        collection.Add(image);
        //    }

        //    image.Dispose();
        //    original.Dispose();
        //    return collection;
            
        //}
    //}
}
