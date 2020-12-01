using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shatter.Core.Structures.Guilds
{
	public class GuildConfig : IGuildData
    {
        [Key]
        public ulong GuildId { get; set; }

        #region GuildConfig
        public string Prefix { get; set; }
        #endregion

        #region Trivia Config
        public bool AllowPublicTriviaGames { get; set; }
        public int TriviaQuestionLimit { get; set; }
        #endregion

        #region Command Config
        public HashSet<string> DisabledModules { get; set; }
        public HashSet<string> DisabledCommands { get; set; }
        public HashSet<string> ActivatedCommands { get; set; }
        #endregion

        public GuildConfig() : this(0, "", true, 10) { }

        public GuildConfig(ulong gid) : this(gid, CoreUtils.Prefix, true, 10) { }

		public GuildConfig(ulong gid, string? p, bool aptg, int tql)
		{
			this.GuildId = gid;

			this.Prefix = p ?? "]";

			this.AllowPublicTriviaGames = aptg;
			this.TriviaQuestionLimit = tql;

			this.ActivatedCommands = new HashSet<string>();
			this.DisabledCommands = new HashSet<string>();
			this.DisabledModules = new HashSet<string>()
            {// Deafult disabled command modules.
                "memberlog",
                "mod",
                "music",
                "filter"
            };
        }
    }
}
