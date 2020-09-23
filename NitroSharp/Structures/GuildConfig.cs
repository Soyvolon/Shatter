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

        public GuildConfig() { }

        public GuildConfig(ulong gid)
        {
            GuildId = gid;
        }

        public GuildConfig(ulong gid, string p)
        {
            GuildId = gid;

            Prefix = p;
        }

    }
}
