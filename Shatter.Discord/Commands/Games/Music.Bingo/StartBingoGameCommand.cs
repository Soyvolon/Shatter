using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

using Newtonsoft.Json;

using Shatter.Core.Structures.Music.Bingo;
using Shatter.Discord.Commands.Attributes;
using Shatter.Discord.Services;

namespace Shatter.Discord.Commands.Games.Music.Bingo
{
    public class StartBingoGameCommand : CommandModule
    {
        private readonly MusicBingoService _bingo;
        private readonly VoiceService _voice;

        public StartBingoGameCommand(MusicBingoService bingo, VoiceService voice)
        {
            this._bingo = bingo;
            this._voice = voice;
        }

        [Command("startbingo")]
        [Description("Starts a new bingo game! You must be in VC and the bot can not be being used by another user in a VC!")]
        [ExecutionModule("bingo")]
        public async Task StartBingoGameCommandAsync(CommandContext ctx)
        {
            if (ctx.Member.VoiceState?.Channel is null)
            {
                await RespondBasicErrorAsync("You need to be in a Voice Channel to use the `startbingo` command!");
                return;
            }

            var conn = await _voice.GetGuildConnection(ctx);

            if (conn is not null)
            {
                if (!(_voice.IsDJ(ctx, out bool _)
                || ctx.Member.PermissionsIn(conn.Channel).HasPermission(Permissions.ManageChannels)))
                {
                    await RespondBasicErrorAsync("You are not the DJ and cannot start a bingo game!");
                    return;
                }
                else
                { // stop anything playing first.
                    _voice.GuildQueues[ctx.Guild.Id].Clear();
                    await conn.StopAsync();
                }
            }

            var globalPath = Path.Join("MusicBingo", "Global");

            HashSet<string>? globalNames = null;
            if(Directory.Exists(globalPath))
            {
                var paths = Directory.GetFiles(globalPath, "*.json", SearchOption.TopDirectoryOnly);
                globalNames = new HashSet<string>();

                foreach (var path in paths)
                    globalNames.Add(Path.GetFileNameWithoutExtension(path));

                globalNames.Remove("example");
            }

            var globalData = globalNames is not null ? "`" + string.Join("`, `", globalNames) + "`" : "No boards found.";

            var localPath = Path.Join("MusicBingo", ctx.Guild.Id.ToString());
            HashSet<string>? localNames = null;
            if(Directory.Exists(localPath))
            {
                var paths = Directory.GetFiles(localPath, "*.json", SearchOption.TopDirectoryOnly);
                localNames = new HashSet<string>();

                foreach(var path in paths)
                    localNames.Add(Path.GetFileNameWithoutExtension(path));
            }

            var localData = localNames is not null ? "`" + string.Join("`, `", localNames) + "`" : "No boards found.";


            var interact = ctx.Client.GetInteractivity();
            var embed = SuccessBase()
                .WithTitle("Musical Bingo!")
                .WithDescription("*Type `cancel` at any time to quit.*\n\n" +
                "Let setup your musical bingo game. Make sure to type the board name exactly as it is show.\n" +
                "*If a global board and a local board share the same name, the local board will be selected over the global one. use `global:`" +
                "before the name of the global board to specify a global board. Ex: `global:example`*\n\n" +
                "Please pick a board to play:")
                .AddField("Global Boards:", globalData)
                .AddField("Local Boards:", localData);

            await ctx.RespondAsync(embed: embed);

            string msg;
            int c = 0;
            do
            {
                if(c++ > 0)
                {
                    await RespondBasicErrorAsync("Failed to find a match. Make sure to type the exact name in.");
                }

                var res = await interact.WaitForMessageAsync(x => x.Author.Id == ctx.Message.Author.Id);

                if (res.TimedOut)
                {
                    await RespondBasicErrorAsync("Bingo setup timed out.");
                    return;
                }
                else if (res.Result.Content.Equals("cancel", System.StringComparison.OrdinalIgnoreCase))
                {
                    await RespondBasicErrorAsync("Aborting setup.");
                    return;
                }

                msg = res.Result.Content.ToLower().Trim();

            } while (!(
                ( // Check to see if it starts with a global: tag, then remove the global and check for a match.
                    msg.StartsWith("global:", System.StringComparison.OrdinalIgnoreCase)
                        && (globalNames?.Contains(msg[7..].Trim()) ?? false)
                ) // if that fails, check for a local match.
                || (localNames?.Contains(msg) ?? false)
                // if that fails, check for a global match.
                || (globalNames?.Contains(msg) ?? false)
            ));

            string finalPath;

            if(msg.StartsWith("global:"))
            {
                msg = msg[7..].Trim();
                finalPath = Path.Join(globalPath, msg + ".json");
            }
            else if(localNames?.Contains(msg) ?? false)
            {
                finalPath = Path.Join(localPath, msg + ".json");
            }
            else
            {
                finalPath = Path.Join(globalPath, msg + ".json");
            }

            using FileStream fs = new(finalPath, FileMode.Open);
            using StreamReader sr = new(fs);
            var json = await sr.ReadToEndAsync();

            var game = JsonConvert.DeserializeObject<MusicBingoGame>(json);

            embed = SuccessBase()
                .WithTitle("Bingo Game Created!")
                .WithDescription($"**Board:** {msg}\n\n" +
                $"Game starting in 15 seconds!");

            await ctx.RespondAsync(embed: embed);

            await Task.Delay(TimeSpan.FromSeconds(15));

            await _bingo.StartGameAsync(game, ctx);
        }
    }
}
