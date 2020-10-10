using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ImageMagick;

namespace LossyBotRewrite
{
    public class ImageWrapper : IImageWrapper
    {
        public MagickImage image;
        public readonly MagickFormat defaultFormat = MagickFormat.Jpg;

        const int defaultFrameCount = 21;

        public ImageWrapper(byte[] data)
        {
            image = new MagickImage(data);
            image.Resize(new Percentage(75));
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
            image.LiquidRescale(new Percentage(65), new Percentage(65));
            image.Resize(new Percentage(135), new Percentage(135));
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
            throw new NotImplementedException();
        }

        public IImageWrapper Dance()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Drift()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Expand()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Explode()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Implode()
        {
            MagickImageCollection collection = new MagickImageCollection();
            for (int i = defaultFrameCount - 1; i >= 0; i--)
            {
                var copy = new MagickImage(image);
                copy.Implode((double)i / 8, PixelInterpolateMethod.Average);
                collection.Add(copy);
            }
            return new GifWrapper(collection);
        }

        public IImageWrapper Lsd()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Spectrum()
        {
            throw new NotImplementedException();
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
