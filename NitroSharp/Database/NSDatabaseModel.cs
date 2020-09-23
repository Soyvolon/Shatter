using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NitroSharp.Structures;

namespace NitroSharp.Database
{
    public class NSDatabaseModel : DbContext
    {
        public DbSet<GuildConfig> Configs { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (Program.Bot is null)
                Program.Bot = new DiscordBot();

            if (Program.Bot.Database is null)
            {
                Task.Run(async () => await Program.Bot.RegisterDatabase()).GetAwaiter().GetResult();
            }

            options.UseNpgsql(Program.Bot.Database.GetConnectionString());
        }
    }
}
