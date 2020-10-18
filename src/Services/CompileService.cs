using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    public class CompileService
    {
        private readonly IConfigurationRoot _config;

        public CompileService(IConfigurationRoot config)
        {
            _config = config;
        }

        public async Task<string> Compile(string code, string lang)
        {
            string json = $@"{{
                ""clientId"": ""{_config["tokens:jdoodle:id"]}"",
                ""clientSecret"": ""{_config["tokens:jdoodle:secret"]}"",
                ""script"": ""{code}"",
                ""language"": ""{lang}"",
                ""versionIndex"": ""0""
            }}";

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.jdoodle.com/execute"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var response = await Globals.httpClient.SendAsync(request))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
        }
    }
}
