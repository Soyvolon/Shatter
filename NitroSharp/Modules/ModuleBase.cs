using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;

namespace NitroSharp.Modules
{
    public abstract class ModuleBase
    {
        public DiscordShardedClient Client { get; protected set; }
        public DiscordRestClient Rest { get; protected set; }

        protected string[] debugTags;

        public ModuleBase(DiscordShardedClient client, DiscordRestClient rest, string[] debugTags = null)
        {
            Client = client;
            Rest = rest;
            this.debugTags = debugTags ?? Array.Empty<string>();
        }

        public abstract Task InitializeAsync();
        public abstract Task StartAsync();
    }
}
