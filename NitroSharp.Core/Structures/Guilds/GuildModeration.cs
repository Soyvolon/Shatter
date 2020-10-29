using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace NitroSharp.Core.Structures.Guilds
{
    public class GuildModeration : IGuildData
    {
        [Key]
        public ulong GuildId { get; set; }

        public ConcurrentDictionary<ulong, DateTime> UserBans { get; set; }

        public ConcurrentDictionary<ulong, DateTime> SlowmodeLocks { get; set; }

        public ConcurrentDictionary<ulong, DateTime> UserMutes { get; set; }

        public ulong? ModLogChannel { get; set; }

        public GuildModeration() { }

        public GuildModeration(ulong gid)
        {
            GuildId = gid;
            UserBans = new ConcurrentDictionary<ulong, DateTime>();
            SlowmodeLocks = new ConcurrentDictionary<ulong, DateTime>();
            UserMutes = new ConcurrentDictionary<ulong, DateTime>();
            ModLogChannel = null;
        }
    }
}
