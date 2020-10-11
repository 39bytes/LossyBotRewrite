using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;

namespace LossyBotRewrite
{
    public class ImageWrapper : IImageWrapper
    {
        public MagickImage image;
        public static MagickFormat defaultFormat = MagickFormat.Jpg;

        public ImageWrapper(byte[] data)
        {
            image = new MagickImage(data);
            image.Resize((Percentage)75);
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
            image.Resize((Percentage)135, (Percentage)135);
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
        public void Dispose()
        {
            image.Dispose();
        }

        public void Write(MemoryStream stream)
        {
            image.Write(stream, defaultFormat);
        }

        public void Write()
        {
            image.Write("lossyimage." + defaultFormat.ToString().ToLower());
        }

        public MagickFormat GetFormat()
        {
            return defaultFormat;
        }
    }
}
