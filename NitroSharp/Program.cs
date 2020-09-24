using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using NitroSharp.Services;

namespace NitroSharp
{
    public class Program
    {
        #region Event Ids
        public static EventId PrefixManager { get; } = new EventId(127001, "Prefix Manager");
        public static EventId CommandResponder { get; } = new EventId(127002, "Command Responder");
        #endregion

        public static readonly string VERSION = "0.0.0";

        public static DiscordBot Bot { get; set; }

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        static void Main(string[] args)
        {
            Bot = new DiscordBot();
            
            Bot.InitializeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Bot.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Task.Delay(-1).ConfigureAwait(false).GetAwaiter().GetResult();
            // Stop the bot from closing itself.
        }
    }
}
