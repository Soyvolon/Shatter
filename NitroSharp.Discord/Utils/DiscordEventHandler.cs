using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NitroSharp.Core.Database;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Discord.Utils
{
    public class DiscordEventHandler
    {
		private readonly DiscordRestClient Rest;
		private readonly DiscordShardedClient Client;
        private readonly ServiceProvider Services;

        public DiscordEventHandler(DiscordShardedClient client, DiscordRestClient rest, ServiceProvider services)
		{
			this.Client = client;
			this.Rest = rest;
            this.Services = services;
		}

		public void Initalize()
		{
			// Register client events.
			Client.Ready += Client_Ready;

            Client.GuildMemberAdded += Client_GuildMemberAdded;
            Client.GuildMemberRemoved += Client_GuildMemberRemoved;
        }

		private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
		{
			Client.Logger.LogInformation(DiscordBot.Event_CommandHandler, "Client Ready!");

			return Task.CompletedTask;
		}

        private async Task Client_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            var eventModel = Services.GetService<NSDatabaseModel>();
            var guild = eventModel.Find<GuildMemberlogs>(e.Guild.Id);

            if (!(guild is null))
            {
                if (!(guild.MemberlogChannel is null) && !(guild.JoinMessage is null))
                {
                    await MemberLogingUtils.SendJoinMessageAsync(guild, e);
                }

                if (!(guild.JoinDmMessage is null))
                {
                    await MemberLogingUtils.SendJoinDMMessage(guild, e);
                }
            }
        }

        private async Task Client_GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            var eventModel = Services.GetService<NSDatabaseModel>();
            var guild = eventModel.Find<GuildMemberlogs>(e.Guild.Id);
            if (!(guild is null))
            {
                if (!(guild.MemberlogChannel is null) && !(guild.LeaveMessage is null))
                {
                    await MemberLogingUtils.SendLeaveMessageAsync(guild, e);
                }
            }
        }
    }
}
