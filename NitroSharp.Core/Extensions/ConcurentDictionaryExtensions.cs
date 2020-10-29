﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using NitroSharp.Core.Database;
using NitroSharp.Core.Structures;
using NitroSharp.Core.Structures.Guilds;

namespace NitroSharp.Core.Extensions
{
    public static class ConcurentDictionaryExtensions
    {
        public static void UpdateOrAddValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, V value, IGuildData cfg, NSDatabaseModel _model)
        {
            dict[key] = value;
            _model.Update(cfg);
        }

        public static bool RemoveValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, IGuildData cfg, NSDatabaseModel _model, out V value)
        {
            _model.Update(cfg);
            return dict.TryRemove(key, out value);
        }
    }
}