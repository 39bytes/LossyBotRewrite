using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace LossyBotRewrite
{
    [Group("voice")]
    [Name("Voice")]
    [Summary("For playing music in voice channels.\nThe bot will leave voice chat automatically if there are no other users left in it.")]
    public class VoiceModule : ModuleBase<SocketCommandContext>
    {
        private readonly VoiceServiceManager _voiceManager;

        public VoiceModule(VoiceServiceManager manager)
        {
            _voiceManager = manager;
        }

        [Command("play")]
        [Summary("Plays a song in your current voice channel or adds it to the queue.\n`voice play [youtube url]` ")]
        public async Task VoicePlay(string url)
        {
            if((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }

            var client = new YoutubeClient();
            var info = await client.Videos.GetAsync(url);
            if (info.Duration.Hours >= 1)
            {
                await ReplyAsync("Video is too long! Keep it under 1 hour.");
                return;
            }

            await ReplyAsync("", false, GetVideoEmbed(info).Build());
            if (_voiceManager.HasActiveService(Context.Guild.Id))
            {
                _voiceManager.AddVideoToServiceQueue(Context.Guild.Id, info);
            }
            else
            {
                await _voiceManager.CreateVoiceService((Context.User as IGuildUser).VoiceChannel, info);
            }
        }

        [Command("playlist")]
        [Summary("Plays an entire youtube playlist.\n`voice playlist [playlist url]`")]
        public async Task VoicePlaylist(params string[] args)
        {
            if ((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }

            bool shuffle = false;
            string playlistUrl;
            if (args[0].ToLower() == "shuffle")
            {
                shuffle = true;
                playlistUrl = args[1];
            }
            else
            {
                playlistUrl = args[0];
            }

            var client = new YoutubeClient();
            var info = await client.Playlists.GetAsync(playlistUrl);
            await ReplyAsync("", false, GetPlaylistEmbed(info).Build());

            var videos = await client.Playlists.GetVideosAsync(info.Id).ToListAsync();

            if (shuffle)
                videos.Shuffle();

            if (_voiceManager.HasActiveService(Context.Guild.Id))
            {
                _voiceManager.AddPlaylistToServiceQueue(Context.Guild.Id, videos);
            }
            else
            {
                await _voiceManager.CreateVoiceService((Context.User as IGuildUser).VoiceChannel, videos);
            }

        }

        [Command("skip")]
        [Summary("Skips the song that's currently playing.\n`voice skip`")]

        public async Task SkipSong()
        {
            if ((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }
            _voiceManager.KillFFMpegProcess(Context.Guild.Id); //Killing the FFMpeg process makes it move on to the next
            await ReplyAsync(":fast_forward: Skipped song");
        }

        [Command("queue")]
        [Summary("Shows the song that's currently playing and the next 9 in the queue.\n`voice queue`")]
        public async Task VoiceQueue()
        {
            if ((Context.User as IGuildUser).VoiceChannel == null)
            {
                await ReplyAsync("Enter a voice channel to use voice commands!");
                return;
            }
            Video playing = _voiceManager.GetCurrentlyPlaying(Context.Guild.Id);
            TimeSpan? uptime = _voiceManager.GetFFMpegUptime(Context.Guild.Id);

            if(!uptime.HasValue)
            {
                await ReplyAsync("No song currently playing!");
                return;
            }

            int playPercent = (int)(100 * uptime.Value.TotalSeconds / playing.Duration.TotalSeconds);
            string progressBar = new string('#', playPercent / 5) + new string('.', 20 - (playPercent / 5));

            string progress = uptime.Value.ToString(@"m\:ss"); 
            string duration = playing.Duration.ToString(@"m\:ss"); 

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(":musical_note: Queue");
            builder.WithThumbnailUrl(playing.Thumbnails.MediumResUrl);
            builder.AddField("Now Playing", $"**{playing.Title}**\n" +
                                            $"`{progressBar}`\n{progress} / {duration}\n" +
                                            $"By: {playing.Author}");

            TimeSpan totalLength = new TimeSpan(); //To calculate total length of the queue

            foreach (Video video in _voiceManager.GetQueue(Context.Guild.Id).Take(9)) //make it restricted to 10 total elements
            {
                string s = video.Duration.ToString(@"m\:ss");
                builder.AddField($"**{video.Title}** ({s})", $"By: {video.Author}");
            }

            foreach(Video video in _voiceManager.GetQueue(Context.Guild.Id))
            {
                totalLength += video.Duration;
            }

            totalLength += playing.Duration - uptime.Value; //Add the remaining time in the current song
            string totalLengthStr = totalLength.ToString(@"hh\:mm\:ss");
            builder.WithFooter($"Queue length: {totalLengthStr}");

            await ReplyAsync("", false, builder.Build());
        }

        [Command("stop")]
        [Summary("Stops sending audio, deletes the queue and leaves voice.\n`voice stop`")]
        public async Task VoiceStop()
        {
            _voiceManager.ForceStopService(Context.Guild.Id);
            await ReplyAsync("Disconnected from voice.");
        }

        private EmbedBuilder GetVideoEmbed(Video video)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(":musical_note: Added song to queue!");
            builder.AddField(video.Title, $"**By:** {video.Author}\n**Length:** {video.Duration.ToString().Substring(3)}");
            builder.WithThumbnailUrl(video.Thumbnails.MediumResUrl);
            return builder;
        }

        private EmbedBuilder GetPlaylistEmbed(Playlist playlist)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(":musical_note: Added playlist to queue!");
            builder.AddField(playlist.Title, $"**By:** {playlist.Author}\n");
            builder.WithThumbnailUrl(playlist.Thumbnails.MediumResUrl);
            return builder;
        }
    }
}
