using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

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

            Bot.InitializeAsync().GetAwaiter().GetResult();

            Bot.StartAsync().GetAwaiter().GetResult();

            Task.Delay(-1).GetAwaiter().GetResult();
            // Stop the bot from closing itself.
        }
    }
}
