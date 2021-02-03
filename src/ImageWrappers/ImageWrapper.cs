using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using ImageMagick.Formats.Caption;

namespace LossyBotRewrite
{
    public class ImageWrapper : IImageWrapper
    {
        public MagickImage image;
        public static MagickFormat defaultFormat = MagickFormat.Jpg;

        public ImageWrapper(byte[] data)
        {
            image = new MagickImage(data);
            if (image.Width > 1000 || image.Height > 1000)
                image.Resize((Percentage)60);
            image.Quality = 50;
        }

        #region Effects

        public void Bulge()
        {
            image.Implode(-1, PixelInterpolateMethod.Average);
        }

        public void Contrast()
        {
            image.BrightnessContrast((Percentage)40, (Percentage)75);
        }

        public void Deepfry()
        {
            image.Resize((Percentage)50);
            image.AddNoise(NoiseType.MultiplicativeGaussian);
            image.Modulate((Percentage)100, (Percentage)300, (Percentage)100);
            image.Resize((Percentage)200);
        }

        public void Edge()
        {
            image.Edge(3);
        }

        public void Haah()
        {
            MagickImage clone = new MagickImage(image);
            clone.Flip();
            clone.Extent(clone.Width, clone.Height / 2);
            image.Composite(clone, CompositeOperator.Over);

            clone.Dispose();
        }

        public void Jpgify()
        {
            image.Quality = 5;
        }

        public void Magik()
        {
            image.LiquidRescale((Percentage)65, (Percentage)65);
            image.Resize((Percentage)153, (Percentage)153);
        }

        public void Negate()
        {
            image.Negate();
        }

        public void Waaw()
        {
            MagickImage clone = new MagickImage(image);
            clone.Flop();
            clone.Extent(clone.Width / 2, clone.Height);
            image.Composite(clone, CompositeOperator.Over);

            clone.Dispose();
        }

        public void Wave()
        {
            image.Wave();
        }

        public void Text(string topText, string bottomText)
        {
            var readSettings = new MagickReadSettings
            {
                StrokeColor = MagickColors.Black,
                StrokeWidth = 1.75,
                FillColor = MagickColors.White,
                BackgroundColor = MagickColors.Transparent,
                FontFamily = "Impact",
                TextGravity = Gravity.North,
                Width = (int)(image.Width * 0.9),
            };

            readSettings.Height = image.Height / (3 + (image.Height / 300));

            image.Alpha(AlphaOption.Opaque);

            if(topText != "")
            {
                using (var top = new MagickImage($"caption:{topText}", readSettings))
                {
                    image.Composite(top, (int)(image.Width * 0.05), 0, CompositeOperator.Over);
                }
            }

            if (bottomText != "")
            {
                readSettings.TextGravity = Gravity.South;
                using (var bottom = new MagickImage($"caption:{bottomText}", readSettings))
                {
                    image.Composite(bottom, (int)(image.Width * 0.05), image.Height - (int)readSettings.Height, CompositeOperator.Over);
                }
            }
        }

        //Gif effects return a new object
        public IImageWrapper Angry()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Angry();
        }

        public IImageWrapper Dance()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Dance();
        }

        public IImageWrapper Drift()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Drift();
        }

        public IImageWrapper Expand()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Expand();
        }

        public IImageWrapper Explode()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Explode();
        }

        public IImageWrapper Implode()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Implode();
        }

        public IImageWrapper Lsd()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Lsd();
        }

        public IImageWrapper Spectrum()
        {
            var gif = new GifWrapper(image);
            image.Dispose();
            return gif.Spectrum();
        }
        #endregion

        public void Write(MemoryStream stream)
        {
            image.Write(stream, defaultFormat);
            image.Dispose();
        }

        public void Write()
        {
            image.Write("lossyimage." + defaultFormat.ToString().ToLower());
        }

        public void Dispose()
        {
            image.Dispose();
        }

        public MagickFormat GetFormat()
        {
            return defaultFormat;
        }
    }
}
