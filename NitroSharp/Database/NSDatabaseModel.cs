using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using NitroSharp.Structures;
using NitroSharp.Structures.Guilds;
using NitroSharp.Structures.Trivia;

namespace NitroSharp.Database
{
    public class NSDatabaseModel : DbContext
    {
        #region Guild Configurations
        public DbSet<GuildConfig> Configs { get; set; }
        public DbSet<GuildFilters> Filters { get; set; }
        public DbSet<GuildModeration> Moderations { get; set; }
        public DbSet<GuildMemberlogs> Memberlogs { get; set; }
        #endregion

        #region User Data
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TriviaPlayer> TriviaPlayers { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (Program.Bot is null)
                Program.Bot = new DiscordBot();

            if (Program.Bot.Database is null)
            {
                Task.Run(async () => await Program.Bot.RegisterDatabase()).GetAwaiter().GetResult();
            }

            options.UseNpgsql(Program.Bot.Database.GetConnectionString())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GuildModeration>()
                .Property(b => b.UserBans)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, DateTime>>(v) ?? new ConcurrentDictionary<ulong, DateTime>());

            modelBuilder.Entity<GuildModeration>()
                .Property(b => b.UserMutes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, DateTime>>(v) ?? new ConcurrentDictionary<ulong, DateTime>());

            modelBuilder.Entity<GuildModeration>()
                .Property(b => b.SlowmodeLocks)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, DateTime>>(v) ?? new ConcurrentDictionary<ulong, DateTime>());

            modelBuilder.Entity<GuildFilters>()
                .Property(b => b.Filters)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<string, Tuple<int, string[]>>>(v) ?? new ConcurrentDictionary<string, Tuple<int, string[]>>());
        }
    }
}
