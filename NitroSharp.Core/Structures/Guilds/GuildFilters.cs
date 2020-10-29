using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitroSharp.Structures.Guilds
{
    public class GuildFilters : IGuildData
    {
        [Key]
        public ulong GuildId { get; set; }

        public ConcurrentDictionary<string, Tuple<int, string[]>> Filters { get; set; }

        [NotMapped]
        HashSet<string> DirectMatches { get; set; } = new HashSet<string>();
        [NotMapped]
        HashSet<string> LookalikeMatches { get; set; } = new HashSet<string>();
        [NotMapped]
        HashSet<string> FoundAnywhereMatches { get; set; } = new HashSet<string>();

        public GuildFilters() { }

        public GuildFilters(ulong gid)
        {
            GuildId = gid;
            Filters = new ConcurrentDictionary<string, Tuple<int, string[]>>();
        }
    }
}