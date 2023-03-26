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

            // DI
            var registrations = new ServiceCollection();
            registrations.AddSingleton<IInvokeClientDiscovery, InvokeClientDiscovery>();
            registrations.AddSingleton<IDiscoveryMaster, DiscoveryMaster>();
            registrations.AddSingleton<IConnector, Connector>();
            var registrar = new TypeRegistrar(registrations);
            
            var app = new CommandApp(registrar);
            app.Configure(config => {
                config.SetApplicationVersion("v0.1");
                config.SetInterceptor(new LogInterceptor());
                config.SetExceptionHandler(ex => {
                    //AnsiConsole.WriteException(ex);
                    AnsiConsole.MarkupLine("[red]Fehler: [/]" + ex.Message);
                });
                
                config.AddCommand<StatusCommand>("status")
                    .WithAlias("ps")
                    .WithDescription("Display server status");

                config.AddBranch<PluginCommand.Settings>("plugin", plugin => {
                    plugin.SetDescription("Work with Plugins");
                    plugin.AddCommand<PluginRunCommand>("status");
                    plugin.AddCommand<PluginUploadCommand>("load");
                    plugin.AddCommand<PluginListCommand>("run")
                        .WithExample(new []{"prime", "300"})
                        .WithExample(new []{"plugin_name", "input"});
                });
            });
            return app.Run(args);
        }
    }
}
