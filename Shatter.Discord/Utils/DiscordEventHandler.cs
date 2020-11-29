using System;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;

namespace Shatter.Discord.Utils
{
	public class DiscordEventHandler : IDisposable
    {
        private readonly DiscordRestClient Rest;
        private readonly DiscordShardedClient Client;
        private readonly ServiceProvider? Services;
        private bool disposedValue;

        public DiscordEventHandler(DiscordShardedClient client, DiscordRestClient rest, ServiceProvider? services)
        {
			this.Client = client;
			this.Rest = rest;
			this.Services = services;
        }

        public void Initalize()
        {
			// Register client events.
			this.Client.Ready += Client_Ready;

			#region Memberlogging Assignment
			this.Client.GuildMemberAdded += MemberLog_GuildMemberAdded;
			this.Client.GuildMemberRemoved += MemberLog_GuildMemberRemoved;
			#endregion

			#region Guild Filters Assignment
			this.Client.MessageCreated += GuildFilters_MessageCreated;
            #endregion
        }
        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
			this.Client.Logger.LogInformation(DiscordBot.Event_CommandHandler, "Client Ready!");

            return Task.CompletedTask;
        }

        #region Memberlogging

        private async Task MemberLog_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
			if (this.Services is null)
			{
				return;
			}

			var eventModel = this.Services.GetRequiredService<ShatterDatabaseContext>();
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

        private async Task MemberLog_GuildMemberRemoved(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
			if (this.Services is null)
			{
				return;
			}

			var eventModel = this.Services.GetRequiredService<ShatterDatabaseContext>();
            var guild = eventModel.Find<GuildMemberlogs>(e.Guild.Id);
            if (!(guild is null))
            {
                if (!(guild.MemberlogChannel is null) && !(guild.LeaveMessage is null))
                {
                    await MemberLogingUtils.SendLeaveMessageAsync(guild, e);
                }
            }
        }
        #endregion

        #region Guild Filters
        public async Task GuildFilters_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot || (e.Author.IsSystem ?? false))
			{
				return;
			}

			if (this.Services is null)
			{
				return;
			}

			var model = this.Services.GetRequiredService<ShatterDatabaseContext>();
            var filter = await model.FindAsync<GuildFilters>(e.Guild.Id);

            if (!(filter is null))
            {
                var cancelSource = new CancellationTokenSource();
                GuildFiltersUtil.FilterUtils[e.Message.Id] = new System.Tuple<Task, CancellationTokenSource>(
                    Task.Run(async () => await GuildFiltersUtil.Run(filter, e, cancelSource.Token)),
                    cancelSource);
            }
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
				// Unregister events
				this.Client.Ready -= Client_Ready;

				#region Memberlogging Assignment
				this.Client.GuildMemberAdded -= MemberLog_GuildMemberAdded;
				this.Client.GuildMemberRemoved -= MemberLog_GuildMemberRemoved;
				#endregion

				#region Guild Filters Assignment
				this.Client.MessageCreated -= GuildFilters_MessageCreated;
                #endregion

                if (disposing)
                {
                    // Stop any in progress operations.
                    foreach(var value in GuildFiltersUtil.FilterUtils.Values)
                    {
                        value.Item2.Cancel();
                    }
                }

				this.disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DiscordEventHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
