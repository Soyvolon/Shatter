using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitroSharp.Structures
{
    public class GuildConfig
    {
        [Key]
        public ulong GuildId { get; set; }

        public string Prefix { get; set; }

        public string Culture { get; set; }

        public GuildConfig() { }

        public GuildConfig(ulong gid)
        {
            GuildId = gid;
            Prefix = Program.Bot.Config.Prefix;
            Culture = "en-US";
        }

        public GuildConfig(ulong gid, string p, string c)
        {
            GuildId = gid;

            Prefix = p;

            Culture = c;
        }

    }
}
