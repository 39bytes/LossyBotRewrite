using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ImageMagick;
using ImageMagick.Formats.Caption;

namespace LossyBotRewrite
{
    public class GifWrapper : IImageWrapper
    {
        public MagickImageCollection image;
        public static MagickFormat defaultFormat = MagickFormat.Gif;

        const int defaultFrameCount = 15;

        public GifWrapper(byte[] data)
        {
            image = new MagickImageCollection(data);
            foreach(var img in image)
            {
                img.Quality = 50;
            }
        }

        public GifWrapper(MagickImageCollection col)
        {
            image = col;
        }
        
        public GifWrapper(MagickImage frame)
        {
            image = new MagickImageCollection();
            for (int i = 0; i < defaultFrameCount; i++)
                image.Add(new MagickImage(frame));
            frame.Dispose();
        }

        #region Effects

        public void Bulge()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Implode(-1, PixelInterpolateMethod.Average);
            }
        }

        public void Contrast()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].BrightnessContrast((Percentage)40, (Percentage)75);
            }
        }

        public void Deepfry()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Resize((Percentage)50);
                image[i].AddNoise(NoiseType.MultiplicativeGaussian);
                image[i].Modulate((Percentage)100, (Percentage)300, (Percentage)100);
                image[i].Resize((Percentage)200);
            }
        }

        public void Edge()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Edge(3);
            }
        }

        public void Haah()
        {
            for (int i = 0; i < image.Count; i++)
            {
                MagickImage clone = new MagickImage(image[i]);
                clone.Flip();
                clone.Extent(clone.Width, clone.Height / 2);
                image[i].Composite(clone, CompositeOperator.Over);

                clone.Dispose();
            }
        }
        

        public void Jpgify()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Quality = 5;
            }
        }

        public void Magik()
        {
            for(int i = 0; i < image.Count; i++)
            {
                image[i].LiquidRescale((Percentage)65, (Percentage)65);
                image[i].Resize((Percentage)135, (Percentage)135);
            }
        }

        public void Negate()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Negate();
            }
        }

        public void Waaw()
        {
            for (int i = 0; i < image.Count; i++)
            {
                MagickImage clone = new MagickImage(image[i]);
                clone.Flop();
                clone.Extent(clone.Width / 2, clone.Height);
                image[i].Composite(clone, CompositeOperator.Over);

                clone.Dispose();
            }
        }

        public void Wave()
        {
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Wave();
            }
        }

        public void Text(string topText, string bottomText)
        {
            var readSettings = new MagickReadSettings
            {
                StrokeColor = MagickColors.Black,
                StrokeWidth = 1.5,
                FillColor = MagickColors.White,
                BackgroundColor = MagickColors.Transparent,
                FontFamily = "Impact",
                TextGravity = Gravity.North,
                Width = image[0].Width,
                Height = image[0].Height / 3,
                Defines = new CaptionReadDefines()
                {
                    MaxFontPointsize = (topText.Length >= 20) ? 40 : 45
                }
            };
            
            if(topText != "")
            {
                using (var top = new MagickImage($"caption:{topText}", readSettings))
                {
                    for (int i = 0; i < image.Count; i++)
                    {
                        image[i].Alpha(AlphaOption.Opaque);
                        image[i].Composite(top, 0, 0, CompositeOperator.Over);
                    }
                }
            }

            if(bottomText != null)
            {
                readSettings.TextGravity = Gravity.South;
                using (var bottom = new MagickImage($"caption:{bottomText}", readSettings))
                {
                    for (int i = 0; i < image.Count; i++)
                    {
                        image[i].Composite(bottom, 0, image[0].Height - (int)readSettings.Height, CompositeOperator.Over);
                    }
                }
            }
        }
        #endregion

        public IImageWrapper Angry()
        {
            Random rand = new Random();
            for (int i = 0; i < image.Count; i++)
            {
                int randW = rand.Next(30) - 15;
                int randH = rand.Next(30) - 15;
                image[i].Roll(randW, randH);
            }
            return this;
        }

        public IImageWrapper Dance()
        {
            int dancePercent = 100;
            int width = (image[0] as MagickImage).Width;
            int height = (image[0] as MagickImage).Height;

            for (int i = image.Count - 1; i >= 0; i--)
            {
                dancePercent += image.Count / 3;
                
                image[i].LiquidRescale((Percentage)dancePercent);
                image[i].Resize(width, height);
            }
            return this;
        }

        public IImageWrapper Drift()
        {
            double percent;
            for (int i = image.Count - 1; i >= 0; i--)
            {
                percent = 100 - (Math.Floor((double)100 / (image.Count + 1)) * i);
                image[i].Resize((Percentage)percent);
            }

            return this;
        }

        public IImageWrapper Expand()
        {
            double percent;
            int width = (image[0] as MagickImage).Width;
            int height = (image[0] as MagickImage).Height;

            for (int i = image.Count - 1; i >= 0; i--)
            {
                percent = 100 - (Math.Floor((double)100 / (image.Count + 1)) * i);
                image[i].LiquidRescale((Percentage)percent);
                image[i].Resize(width, height);
                
            }
            return this;
        }

        public IImageWrapper Explode()
        {
            for (int i = image.Count - 1; i >= 0; i--)
            {
                image[i].Implode((double)-i / 3, PixelInterpolateMethod.Average);
            }

            return this;
        }

        public IImageWrapper Implode()
        {
            for (int i = image.Count - 1; i >= 0; i--)
            {
                image[i].Implode((double)i / 8, PixelInterpolateMethod.Average);
            }
            return this;
        }

        public IImageWrapper Lsd()
        {
            double i_the_prequel = 100;
            double step = 200.0 / image.Count;
            for (int i = 0; i < image.Count; i++)
            {
                image[i].ColorFuzz = (Percentage)20;
                image[i].Settings.FillColor = MagickColors.Yellow;
                image[i].Opaque(MagickColors.Yellow, MagickColors.White);
                image[i].Settings.FillColor = MagickColors.Blue;
                image[i].Opaque(MagickColors.Yellow, MagickColors.Black);
                image[i].Modulate((Percentage)100, (Percentage)300, (Percentage)i_the_prequel);
                if (i_the_prequel + step > 200)
                    i_the_prequel -= 200;
                i_the_prequel += step;
            }
            return this;
        }

        public IImageWrapper Spectrum()
        {
            double i_the_prequel = 100; //legendary variable name so i had to keep it
            double step = 200.0 / image.Count;
            for (int i = 0; i < image.Count; i++)
            {
                image[i].Modulate((Percentage)100, (Percentage)100, (Percentage)i_the_prequel);
                if (i_the_prequel + step > 200)
                    i_the_prequel -= 200;
                i_the_prequel += step;
            }
            return this;
        }

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
