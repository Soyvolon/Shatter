﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using NitroSharp.Core;
using NitroSharp.Core.Structures;
using NitroSharp.Core.Structures.Guilds;
using NitroSharp.Core.Structures.Trivia;

namespace NitroSharp.Core.Database
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

        protected override async void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbConfig = await ConfigurationManager.RegisterDatabase(null);

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
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<string, Tuple<int, string[]>>>(v) ?? new ConcurrentDictionary<string, Tuple<int, string[]>>());
        }
    }
}
