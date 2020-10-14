using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace LossyBotRewrite
{
    public static class Globals
    {
        public const string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "lossy" + Path.DirectorySeparatorChar;

        public static HttpClient httpClient = new HttpClient();
    }
}
