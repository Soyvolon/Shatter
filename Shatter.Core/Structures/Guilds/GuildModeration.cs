using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Shatter.Core.Structures.Guilds
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
			this.GuildId = gid;
			this.UserBans = new ConcurrentDictionary<ulong, DateTime>();
			this.SlowmodeLocks = new ConcurrentDictionary<ulong, DateTime>();
			this.UserMutes = new ConcurrentDictionary<ulong, DateTime>();
			this.ModLogChannel = null;
        }
    }
}
