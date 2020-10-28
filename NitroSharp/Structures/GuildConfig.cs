using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

using Google.Apis.YouTube.v3;

namespace NitroSharp.Structures
{
    public class GuildConfig
    {
        [Key]
        public ulong GuildId { get; set; }

        #region GuildConfig
        public string Prefix { get; set; }

        public string Culture { get; set; }
        #endregion

        #region Trivia Config
        public bool AllowPublicTriviaGames { get; set; }
        public int TriviaQuestionLimit { get; set; }
        #endregion

        #region Member Log
        public string? JoinDmMessage { get; set; }

        public ulong? MemberlogChannel { get; set; }
        public MemberlogMessage? JoinMessage { get; set; }
        public MemberlogMessage? LeaveMessage { get; set; }
        #endregion

        #region Moderation
        public ConcurrentDictionary<ulong, DateTime> UserBans { get; set; }
        [NotMapped]
        public ConcurrentDictionary<ulong, DateTime> SlowmodeLocks { get; set; }
        [NotMapped]
        public ConcurrentDictionary<ulong, DateTime> UserMutes { get; set; }
        #endregion

        public GuildConfig() { }

        public GuildConfig(ulong gid)
        {
            GuildId = gid;
            Prefix = Program.Bot.Config.Prefix;
            Culture = "en-US";
            AllowPublicTriviaGames = true;
            TriviaQuestionLimit = 10;
            JoinDmMessage = null;
            MemberlogChannel = null;
            JoinMessage = null;
            LeaveMessage = null;
            UserBans = new ConcurrentDictionary<ulong, DateTime>();
        }

        public GuildConfig(ulong gid, string p, string c, bool aptg, int tql, string? joinDmMsg, ulong? mlogchan, MemberlogMessage? jmsg, MemberlogMessage? lmsg, ConcurrentDictionary<ulong, DateTime> bans)
        {
            GuildId = gid;

            Prefix = p;

            Culture = c;

            AllowPublicTriviaGames = aptg;
            TriviaQuestionLimit = tql;

            JoinDmMessage = joinDmMsg;
            MemberlogChannel = mlogchan;
            JoinMessage = jmsg;
            LeaveMessage = lmsg;
            UserBans = bans;
        }

    }
}
