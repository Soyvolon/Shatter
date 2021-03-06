using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Music
{
	public class MusicListCommand : CommandModule
    {
        private readonly VoiceService _voice;

        public MusicListCommand(VoiceService voice)
        {
			this._voice = voice;
        }

        [Command("musiclist")]
        [Description("Shows the currently playing song along with any queued songs.")]
        [Aliases("nowplaying")]
        [ExecutionModule("music")]
        public async Task MusicListCommandAsync(CommandContext ctx)
        {
            var conn = await this._voice.GetGuildConnection(ctx);

            if (conn is null)
            {
                await RespondBasicErrorAsync("I'm not connected to any Voice Channels!");
                return;
            }


            var nowPlaying = conn.CurrentState.CurrentTrack;
            string data = $":green_circle: :notes:] {nowPlaying.Title} by {nowPlaying.Author} - `{conn.CurrentState.PlaybackPosition:mm\\:ss}/{nowPlaying.Length:mm\\:ss}`";

            if (this._voice.GuildQueues.TryGetValue(ctx.Guild.Id, out var queue))
            {
                int i = 1;
                int last = queue.Count;
                foreach (var track in queue)
                {
                    if (i == 1)
					{
						data += $"\n:yellow_circle: **{i}]** {track.Title} by {track.Author} - `{track.Length:mm\\:ss}`";
					}
					else if (i == last)
					{
						data += $"\n:red_circle: **{i}]** {track.Title} by {track.Author} - `{track.Length:mm\\:ss}`";
					}
					else
					{
						data += $"\n:black_circle: **{i}]** {track.Title} by {track.Author} - `{track.Length:mm\\:ss}`";
					}

					i++;
                }
            }

            var interact = ctx.Client.GetInteractivity();

            var pages = interact.GeneratePagesInEmbed(data, SplitType.Line, CommandModule.SuccessBase());

            var emojis = new PaginationEmojis()
            {
                Left = DiscordEmoji.FromName(ctx.Client, ":arrow_up_small:"),
                SkipLeft = DiscordEmoji.FromName(ctx.Client, ":arrow_double_up:"),
                Right = DiscordEmoji.FromName(ctx.Client, ":arrow_down_small:"),
                SkipRight = DiscordEmoji.FromName(ctx.Client, ":arrow_double_down:"),
                Stop = DiscordEmoji.FromName(ctx.Client, ":stop_button:"),
            };

            await interact.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages, emojis);
        }
    }
}
