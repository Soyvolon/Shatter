using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using NitroSharp.Structures;
using NitroSharp.Structures.Trivia;

namespace NitroSharp.Database
{
    public class NSDatabaseModel : DbContext
    {
        public DbSet<GuildConfig> Configs { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TriviaPlayer> TriviaPlayers { get; set; }

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

            modelBuilder.Entity<GuildConfig>()
                .Property(b => b.UserBans)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, DateTime>>(v));
        }
    }
}
