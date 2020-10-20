using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LossyBotRewrite
{
    public class SplatnetService
    {
        public async Task<(JToken token, int currentRotation)> GetScheduleAsync()
        {
            DirectoryInfo dir = new DirectoryInfo(Globals.path);
            var files = dir.GetFiles("schedules-*.json");
            int currentRotation = 0;
            if (!files.Any())
            {
                await DownloadSchedule();
            }
            else
            {
                var currentSchedule = files[0];
                long timestamp = long.Parse(Path.GetFileNameWithoutExtension(currentSchedule.Name).Split('-')[1]); //Gets the timestamp part of the file name
                long timeSinceLast = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timestamp;
                if (timeSinceLast >= 7200 * 8) //if 16 hours have passed since the last rotation
                {
                    await DownloadSchedule();
                    File.Delete(Globals.path + currentSchedule.Name);
                }
                else
                {
                    currentRotation = (int)(timeSinceLast / 7200);
                }
            }
            files = dir.GetFiles("schedules-*.json");
            string data = await File.ReadAllTextAsync(Globals.path + files[0].Name);
            return (JToken.Parse(data), currentRotation);
        }

        private async Task DownloadSchedule()
        {
            string response = await Globals.httpClient.GetStringAsync("https://splatoon2.ink/data/schedules.json");
            JToken token = JToken.Parse(response);
            long timestamp = token["gachi"][0].Value<long>("start_time");
            using (StreamWriter sw = File.CreateText(Globals.path + $"schedules-{timestamp}.json"))
            {
                await sw.WriteAsync(response);
            }
        }
    }
}
