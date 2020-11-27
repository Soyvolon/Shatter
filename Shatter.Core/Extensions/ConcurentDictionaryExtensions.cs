using System.Collections.Concurrent;

using Shatter.Core.Database;
using Shatter.Core.Structures.Guilds;

namespace Shatter.Core.Extensions
{
	public static class ConcurentDictionaryExtensions
    {
        public static void UpdateOrAddValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, V value, IGuildData cfg, ShatterDatabaseContext _model)
        {
            _model.Update(cfg);
            dict[key] = value;
        }

        public static bool RemoveValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, IGuildData cfg, ShatterDatabaseContext _model, out V value)
        {
            _model.Update(cfg);
            return dict.TryRemove(key, out value);
        }
    }
}