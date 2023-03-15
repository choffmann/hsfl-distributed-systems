using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.Client.Commands;
using hsfl.ceho5518.vs.server.ConcreatService;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using hsfl.ceho5518.vs.Client.Injections;

namespace hsfl.ceho5518.vs.Client {
    internal class Program {
        static int Main(string[] args) {
            // Set Text to UTF-8 to use Emojis and Spinners
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Set LogLevel
            Logger.Instance.LogLevel = LogLevel.Info;
            
            // DI
            var registrations = new ServiceCollection();
            registrations.AddSingleton<IClientDiscoveryService, ClientDiscoveryService>();
            registrations.AddSingleton<IDiscoveryMaster, DiscoveryMaster>();
            var registrar = new TypeRegistrar(registrations);
            
            var app = new CommandApp(registrar);
            app.Configure(config => {
                config.SetApplicationVersion("v0.1");
                
                config.AddCommand<StatusCommand>("status")
                    .WithAlias("ps")
                    .WithDescription("Display server status");
            });
            return app.Run(args);
        }
    }
}
