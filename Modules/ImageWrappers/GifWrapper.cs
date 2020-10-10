using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageMagick;

namespace LossyBotRewrite
{
    public class GifWrapper : IImageWrapper
    {
        public MagickImageCollection image;
        public readonly MagickFormat defaultFormat = MagickFormat.Gif;

        public GifWrapper(byte[] data)
        {
            image = new MagickImageCollection(data);
        }

        public GifWrapper(MagickImageCollection col)
        {
            image = col;
        }

        #region Effects
        public IImageWrapper Angry()
        {
            throw new NotImplementedException();
        }

        public void Bulge()
        {
            throw new NotImplementedException();
        }

        public void Contrast()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Dance()
        {
            throw new NotImplementedException();
        }

        public void Deepfry()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Drift()
        {
            throw new NotImplementedException();
        }

        public void Edge()
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

        public void Haah()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Implode()
        {
            for (int i = image.Count - 1; i >= 0; i--)
            {
                image[i].Implode((double)i / 8, PixelInterpolateMethod.Average);
            }
            return this;
        }

        public void Jpgify()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Lsd()
        {
            throw new NotImplementedException();
        }

        public void Magik()
        {
            for(int i = 0; i < image.Count; i++)
            {
                image[i].LiquidRescale(new Percentage(65), new Percentage(65));
                image[i].Resize(new Percentage(135), new Percentage(135));
            }
        }

        public void Negate()
        {
            throw new NotImplementedException();
        }

        public IImageWrapper Spectrum()
        {
            throw new NotImplementedException();
        }

        public void Waaw()
        {
            throw new NotImplementedException();
        }

        public void Wave()
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
