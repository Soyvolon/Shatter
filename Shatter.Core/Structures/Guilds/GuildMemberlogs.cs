using System.ComponentModel.DataAnnotations;

namespace Shatter.Core.Structures.Guilds
{
	public class GuildMemberlogs : IGuildData
    {
        [Key]
        public ulong GuildId { get; set; }

        public string? JoinDmMessage { get; set; }

        public ulong? MemberlogChannel { get; set; }
        public MemberlogMessage? JoinMessage { get; set; }
        public MemberlogMessage? LeaveMessage { get; set; }

        public GuildMemberlogs() { }

        public GuildMemberlogs(ulong gid)
        {
			this.GuildId = gid;
			this.JoinDmMessage = null;
			this.MemberlogChannel = null;
			this.JoinMessage = null;
			this.LeaveMessage = null;
        }
    }
}
