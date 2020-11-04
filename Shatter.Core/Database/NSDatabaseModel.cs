using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Newtonsoft.Json;

using Shatter.Core.Structures;
using Shatter.Core.Structures.Guilds;
using Shatter.Core.Structures.Trivia;

namespace Shatter.Core.Database
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
            var dbConfig = ConfigurationManager.RegisterDatabase(null).GetAwaiter().GetResult();

            if (dbConfig is null)
                throw new NullReferenceException("DB Config cannont be null!");

            options.UseNpgsql(dbConfig.GetConnectionString())
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
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<string, Tuple<int, HashSet<string>>>>(v) ?? new ConcurrentDictionary<string, Tuple<int, HashSet<string>>>());

            modelBuilder.Entity<GuildFilters>()
                .Property(b => b.BypassFilters)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, HashSet<string>>>(v) ?? new ConcurrentDictionary<ulong, HashSet<string>>());

            modelBuilder.Entity<GuildConfig>()
                .Property(b => b.ActivatedCommands)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<HashSet<string>>(v) ?? new HashSet<string>());

            modelBuilder.Entity<GuildConfig>()
                .Property(b => b.DisabledCommands)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<HashSet<string>>(v) ?? new HashSet<string>());

            modelBuilder.Entity<GuildConfig>()
                .Property(b => b.DisabledModules)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<HashSet<string>>(v) ?? new HashSet<string>()
                    {// Deafult disabled command modules.
                        "memberlog",
                        "mod",
                        "music",
                        "filter"
                    });
        }
    }
}
