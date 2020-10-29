using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;

namespace NitroSharp.Core.Structures.Guilds
{
    public class GuildFilters : IGuildData
    {
        public const string AllFilters = "A";
        public static readonly Regex regex = new Regex("^[a-zA-z]*$");

        [Key]
        public ulong GuildId { get; set; }

        public ConcurrentDictionary<string, Tuple<int, HashSet<string>>> Filters { get; set; }
        public ConcurrentDictionary<ulong, HashSet<string>> BypassFilters { get; set; }

        [NotMapped]
        public HashSet<string> DirectMatches
        {
            get
            {
                var set = new HashSet<string>();
                var data = Filters.Where(x => x.Value.Item1 == 1);
                foreach (var ary in data)
                {
                    set.UnionWith(ary.Value.Item2);
                }
                return set;
            }
        }

        [NotMapped]
        public HashSet<string> FoundAnywhereMatches
        {
            get
            {
                var set = new HashSet<string>();
                var data = Filters.Where(x => x.Value.Item1 == 2);
                foreach (var ary in data)
                {
                    set.UnionWith(ary.Value.Item2);
                }
                return set;
            }
        }

        public GuildFilters() { }

        public GuildFilters(ulong gid)
        {
            GuildId = gid;
            Filters = new ConcurrentDictionary<string, Tuple<int, HashSet<string>>>();
            BypassFilters = new ConcurrentDictionary<ulong, HashSet<string>>();
        }
    }
}