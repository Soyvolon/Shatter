using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Shatter.Core.Structures.Guilds;
using Shatter.Core.Utils;
using Shatter.Discord.Commands;

namespace Shatter.Discord.Utils
{
	public static class MemberLogingUtils
    {
        public static async Task<string> ReplaceValues(string message, GuildMemberAddEventArgs e)
        {
            return await ReplaceValues(message,
                e.Member.Username,
                e.Guild.Name,
                e.Guild.MemberCount.ToString());
        }

        public static async Task<string> ReplaceValues(string message, GuildMemberRemoveEventArgs e)
        {
            return await ReplaceValues(message,
                e.Member.Username,
                e.Guild.Name,
                e.Guild.MemberCount.ToString());
        }

        public static async Task<string> ReplaceValues(string message, CommandContext ctx)
        {
            return await ReplaceValues(message,
                ctx.Member.Username,
                ctx.Guild.Name,
                ctx.Guild.MemberCount.ToString());
        }

        public static Task<string> ReplaceValues(string message, string user, string guild, string count)
        {
            var msg = message.Replace("{server}", guild);
            msg = msg.Replace("{guild}", guild);

            msg = msg.Replace("{user}", user);
            msg = msg.Replace("{member}", user);

            msg = msg.Replace("{membercount}", count);
            return Task.FromResult(msg);
        }

        public static async Task SendJoinMessageAsync(GuildMemberlogs g, CommandContext ctx)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.JoinMessage?.Message is null))
                    msg = await ReplaceValues(g.JoinMessage.Message, ctx);

                await SendJoinMessageAsync(g, msg, ctx.Member.Username, ctx.Member?.AvatarUrl ?? "");
            }
        }

        public static async Task SendJoinMessageAsync(GuildMemberlogs g, GuildMemberAddEventArgs e)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.JoinMessage?.Message is null))
                    msg = await ReplaceValues(g.JoinMessage.Message, e);

                await SendJoinMessageAsync(g, msg, e.Member.Username, e.Member?.AvatarUrl ?? "");
            }
        }

        public static async Task SendJoinMessageAsync(GuildMemberlogs g, string? msg, string username, string avatarUrl)
        {
            if (g.JoinMessage?.IsEmbed ?? false)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor(CommandModule.Colors[ColorType.Memberlog][0]),
                    Description = msg is null ? "" : msg,
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        IconUrl = avatarUrl,
                        Text = "User Joined"
                    },
                    Timestamp = DateTime.Now
                };

                try
                {
                    await DiscordBot.Bot.Rest.CreateMessageAsync((ulong)g.MemberlogChannel, "", false, embed, null);
                }
                catch { } // ignore
            }
            else if (g.JoinMessage?.IsImage ?? false)
            {
                using var stream = await SvgHandler.GetWelcomeImage(true, username, avatarUrl);
                if (!(stream is null))
                {
                    try
                    {
                        await DiscordBot.Bot.Rest.UploadFileAsync((ulong)g.MemberlogChannel, stream, "welcome-message.png", "", false, null, null);
                    }
                    catch { } // ignore
                }
            }
            else if (!(msg is null))
            {
                try
                {
                    await DiscordBot.Bot.Rest.CreateMessageAsync((ulong)g.MemberlogChannel, msg, false, null, null);
                }
                catch { } // ignore
            }
        }

        public static async Task SendLeaveMessageAsync(GuildMemberlogs g, CommandContext ctx)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.LeaveMessage?.Message is null))
                    msg = await ReplaceValues(g.LeaveMessage.Message, ctx);

                await SendLeaveMessageAsync(g, msg, ctx.Member.Username, ctx.Member?.AvatarUrl ?? "");
            }
        }

        public static async Task SendLeaveMessageAsync(GuildMemberlogs g, GuildMemberRemoveEventArgs e)
        {
            if (!(g.MemberlogChannel is null))
            {
                string? msg = null;
                if (!(g.LeaveMessage?.Message is null))
                    msg = await ReplaceValues(g.LeaveMessage.Message, e);

                await SendLeaveMessageAsync(g, msg, e.Member.Username, e.Member?.AvatarUrl ?? "");
            }
        }

        public static async Task SendLeaveMessageAsync(GuildMemberlogs g, string? msg, string username, string avatarUrl)
        {
            if (g.LeaveMessage?.IsEmbed ?? false)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor(CommandModule.Colors[ColorType.Memberlog][1]),
                    Description = msg is null ? "" : msg,
                    Footer = new DiscordEmbedBuilder.EmbedFooter()
                    {
                        IconUrl = avatarUrl,
                        Text = "User Left"
                    },
                    Timestamp = DateTime.Now
                };

                try
                {
                    await DiscordBot.Bot.Rest.CreateMessageAsync((ulong)g.MemberlogChannel, "", false, embed, null);
                }
                catch { } // ignore

            }
            else if (g.LeaveMessage?.IsImage ?? false)
            {
                using var stream = await SvgHandler.GetWelcomeImage(false, username, avatarUrl);
                if (!(stream is null))
                {
                    try
                    {
                        await DiscordBot.Bot.Rest.UploadFileAsync((ulong)g.MemberlogChannel, stream, "farewell-message.png", "", false, null, null);
                    }
                    catch { } // ignore
                }
            }
            else if (!(g.LeaveMessage?.Message is null))
            {
                try
                {
                    await DiscordBot.Bot.Rest.CreateMessageAsync((ulong)g.MemberlogChannel, msg, false, null, null);
                }
                catch { } // ignore
            }
        }

        public static async Task SendJoinDMMessage(GuildMemberlogs g, GuildMemberAddEventArgs e)
        {
            if (g.JoinDmMessage is null) return;

            var msg = await ReplaceValues(g.JoinDmMessage, e);
            try
            {
                var dms = await e.Member.CreateDmChannelAsync();

                await dms.SendMessageAsync(msg);
            }
            catch { } // ignore
        }
    }
}
