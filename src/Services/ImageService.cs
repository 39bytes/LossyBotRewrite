using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ImageMagick;
using YoutubeExplode.Videos.Streams;

namespace LossyBotRewrite
{
    public class ImageService
    {
        public async Task<IImageWrapper?> ProcessImageAsync(string url, string[] args)
        {
            IImageWrapper? img;

            if (url.Contains("tenor.com"))
            {
                string tenorGif = await GetTenorImageUrl(url);
                img = new GifWrapper(await DownloadImageAsync(tenorGif));
            }
            else if (url.Contains(".gif"))
                img = new GifWrapper(await DownloadImageAsync(url));
            else
                img = new ImageWrapper(await DownloadImageAsync(url));

            if (args[0].ToLower() == "text")
            {
                string top = "";
                string bottom = "";
                if (args.Length > 1)
                    top = args[1];
                if (args.Length > 2)
                    bottom = args[2];
                img.Text(top, bottom);
                return img;
            }

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
                    case "drift":
                        img = img.Drift();
                        break;
                    case "expand":
                        img = img.Expand();
                        break;
                    case "explode":
                        img = img.Explode();
                        break;
                    case "dance":
                        img = img.Dance();
                        break;
                    case "angry":
                        img = img.Angry();
                        break;
                    case "spectrum":
                        img = img.Spectrum();
                        break;
                    case "lsd":
                        img = img.Lsd();
                        break;
                    default:
                        throw new Exception($"Invalid effect `{effect}`");
                }
            }
            return img;
        }

        public async Task ProcessWhatVideoAsync(string imageUrl, long now)
        {
            byte[] data = await DownloadImageAsync(imageUrl);
            string videoPath = Globals.path + "what.mp4";
            int height;
            int width;
            using (MagickImage img = new MagickImage(data))
            {
                img.Resize(new MagickGeometry(432, 243));
                height = img.Height;
                width = img.Width;
                img.Write($"{now}.jpg", MagickFormat.Jpg);
            }

            int topLeftX = (432 - width) / 2 + 24;
            int topLeftY = (243 - height) / 2 + 43;

            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i {videoPath} -strict 2 -i {now}.jpg -filter_complex \"[0:v][1:v] overlay={topLeftX}:{topLeftY}:enable='between(t,0,8.2)'\" " +
                            $"-c:a copy {now}.mp4",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }
        public async Task AddFnafSoundAsync(string imageUrl, long now)
        {
            byte[] data = await DownloadImageAsync(imageUrl);
            string audioPath = Globals.path + "fnaf.mp3";
            using (MagickImage img = new MagickImage(data))
            {
                img.Write($"{now}fnaf.png", MagickFormat.Png);
            }
            var processInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-loop 1 -y -i {now}fnaf.png -i {audioPath} -t 6 -vsync vfr {now}temp.mp4",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
            //var thumbnailProcessInfo = new ProcessStartInfo
            //{
            //    FileName = "ffmpeg",
            //    Arguments = $"-i {now}temp.mp4 -i {now}fnaf.png -map 1 -map 0 -c copy -disposition:0 attached_pic {now}fnaf.mp4",
            //    UseShellExecute = true,
            //    WindowStyle = ProcessWindowStyle.Hidden
            //};
            //using (var process = Process.Start(thumbnailProcessInfo))
            //{
            //    process.WaitForExit();
            //}
        }

        public async Task<Stream> WriteToStream(IImageWrapper img)
        {
            MemoryStream stream = new MemoryStream();
            await Task.Run(() => img.Write(stream));
            stream.Position = 0;
            return stream;
        }

        private async Task<byte[]> DownloadImageAsync(string url)
        {
            byte[] data = await Globals.httpClient.GetByteArrayAsync(url);
            return data;
        }

        private async Task<string> GetTenorImageUrl(string tenorLink)
        {
            using (var response = await Globals.httpClient.GetAsync(tenorLink))
            {
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(result);
                    var node = document.DocumentNode.SelectSingleNode("/html/body/div/div/div[2]/div/div[1]/div[1]/div/div/div/div/img");
                    return node.GetAttributeValue("src", "");
                }
            }
        }
    }
}
