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
            GuildId = gid;
            JoinDmMessage = null;
            MemberlogChannel = null;
            JoinMessage = null;
            LeaveMessage = null;
        }
    }
}
