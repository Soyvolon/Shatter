﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using NitroSharp.Database;
using NitroSharp.Structures;

namespace NitroSharp.Extensions
{
    public static class ConcurentDictionaryExtensions
    {
        public static void UpdateOrAddValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, V value, GuildConfig cfg, NSDatabaseModel _model)
        {
            dict[key] = value;
            _model.Update(cfg);
        }

        public static bool RemoveValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, GuildConfig cfg, NSDatabaseModel _model, out V value)
        {
            _model.Update(cfg);
            return dict.TryRemove(key, out value);
        }
    }
}