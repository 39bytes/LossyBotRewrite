using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace LossyBotRewrite
{
    [Name("Tweet")]
    [Group("tweet")]
    public class TwitterModule : ModuleBase<SocketCommandContext>
    {
        [Command("recent")]
        public async Task TweetRecent(string id)
        {
            var latestTweet = Timeline.GetUserTimeline(id, 1).ToArray();
            await ReplyAsync(Context.User.Mention + " " + latestTweet[0].Url);
        }

        [Command("random")]
        public async Task TweetRandom(string id)
        {
            Random rand = new Random();
            var p = new UserTimelineParameters();
            p.ExcludeReplies = true;
            p.MaximumNumberOfTweetsToRetrieve = 40;
            var latestTweets = Timeline.GetUserTimeline(id, p).ToArray();

            int random = 0;
            int count = 0;
            do
            {
                random = rand.Next(latestTweets.Count());
                count++;
                if (count >= 40) break;
            } while (latestTweets[random].IsRetweet);

            if(count >= 40)
            {
                await ReplyAsync("Failed to get random tweet.");
                return;
            }
            await ReplyAsync(Context.User.Mention + " " + latestTweets[random].Url);
        }

        [Command("popular")]
        public async Task TweetPopular(string id)
        {
            var user = Timeline.GetUserTimeline(id).ToArray();

            int max = 0;
            string maxUrl = "User not found";

            foreach (var tweet in user)
            {
                int current = tweet.FavoriteCount + tweet.RetweetCount;
                if (current > max && !tweet.IsRetweet)
                {
                    max = current;
                    maxUrl = tweet.Url;
                }
            }

            await ReplyAsync(Context.User.Mention + " " + maxUrl);
        }
    }
}
