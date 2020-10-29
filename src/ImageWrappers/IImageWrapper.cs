using ImageMagick;
using System;
using System.IO;

namespace LossyBotRewrite
{
    public interface IImageWrapper
    {
        public void Magik();
        public void Edge();
        public void Wave();
        public void Jpgify();
        public void Deepfry();
        public void Waaw();
        public void Haah();
        public void Contrast();
        public void Negate();
        public void Bulge();
        public void Text(string topText, string bottomText); 
        public IImageWrapper Expand();
        public IImageWrapper Drift();
        public IImageWrapper Implode();
        public IImageWrapper Explode();
        public IImageWrapper Dance();
        public IImageWrapper Angry();
        public IImageWrapper Spectrum();
        public IImageWrapper Lsd();

        public void Dispose();

        public void Write(MemoryStream stream);

        public void Write();

        public MagickFormat GetFormat();
    }
}
