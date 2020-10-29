using System.Collections.Concurrent;

using NitroSharp.Core.Database;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Core.Extensions
{
    public static class ConcurentDictionaryExtensions
    {
        public static void UpdateOrAddValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, V value, IGuildData cfg, NSDatabaseModel _model)
        {
            _model.Update(cfg);
            dict[key] = value;
        }

        public static bool RemoveValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, IGuildData cfg, NSDatabaseModel _model, out V value)
        {
            _model.Update(cfg);
            return dict.TryRemove(key, out value);
        }
    }
}