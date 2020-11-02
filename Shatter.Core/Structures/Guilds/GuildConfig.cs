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

        public GuildConfig() { }

        public GuildConfig(ulong gid)
        {
            GuildId = gid;
            Prefix = CoreUtils.Prefix;
            AllowPublicTriviaGames = true;
            TriviaQuestionLimit = 10;
        }

        public GuildConfig(ulong gid, string p, string c, bool aptg, int tql)
        {
            GuildId = gid;

            Prefix = p;

            AllowPublicTriviaGames = aptg;
            TriviaQuestionLimit = tql;
        }
    }
}
