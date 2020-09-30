using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace LossyBotRewrite
{
    public static class Globals
    {
        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "lossy";

        public static HttpClient httpClient = new HttpClient();
    }
}
