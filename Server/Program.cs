using hsfl.ceho5518.vs.server.DiscoveryProxy;
using hsfl.ceho5518.vs.server.Plugins;
using hsfl.ceho5518.vs.server.Sate;
using hsfl.ceho5518.vs.server.LoggerService;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using hsfl.ceho5518.vs.server.ConcreatService;

namespace hsfl.ceho5518.vs.server {
    internal class Program {
        static void Main(string[] args) {
            StartUp();

            SetupDiscovery();

            // Load Plugins
            LoadPlugins();

            // Initialization done
            Logger.SuccessEmoji("Initialization of System successfully");
            Logger.Info("Waiting for a job...");

            Console.ReadLine();
        }

        private static void StartUp() {
            // Set Text to UTF-8 to use Emojis and Spinners
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Set LogLevel
            Logger.LogLevel = LogLevel.Info;

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .Start(
                $"[yellow]Starting Server...[/]", ctx => {
                    Thread.Sleep(2000);
                    Logger.Info("Starting Server...");
                });
        }

        private static void SetupDiscovery() {
            DiscoveryProxy.DiscoveryProxy proxy = new DiscoveryProxy.DiscoveryProxy(new Uri("net.tcp://localhost:8001/Probe"));
            proxy.SetupProxy();
        }

        private static void LoadPlugins() {
            PluginService pluginService = new PluginService("C:\\Users\\hoffmann\\Documents\\FH Flensburg\\7. Semester\\Verteilte Systeme\\VS-Hausarbeit\\DemoPlugin\\bin\\Debug");
        }
    }
}
