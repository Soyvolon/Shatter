using System;

using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace NitroSharp
{
    public class Program
    {
        public static int Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();

            app.HelpOption("-h|--help");

            app.OnExecute(async () =>
            {
                

                return 0;
            });

            return app.Execute(args);
        }
    }
}
