using hsfl.ceho5518.vs.server.DiscoveryProxy;
using hsfl.ceho5518.vs.server.Plugins;
using hsfl.ceho5518.vs.server.Sate;
using hsfl.ceho5518.vs.LoggerService;
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
using System.IO;
using System.Diagnostics;
using System.Timers;
using hsfl.ceho5518.vs.server.ServiceContracts;

namespace hsfl.ceho5518.vs.server {
    internal class Program {
        private static ILogger logger = Logger.Instance;
        static void Main(string[] args) {
            Directory.CreateDirectory(@".\Logs");
            StartUp();
            SetupDiscovery();
            // Load Plugins
            LoadPlugins();

            // Initialization done
            logger.Success("Systeme wurde erflogreich initialisiert");
            logger.Info("Warte auf einen Job...");

            GlobalState.GetInstance().ServerStatus = ServerStatus.IDLE;
            
            Console.ReadLine();
            OnExit();
        }

        private static void StartUp() {
            // Set Text to UTF-8 to use Emojis and Spinners
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Set LogLevel
            logger.LogLevel = LogLevel.Debug;

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .Start(
                    $"[yellow]Starte Server...[/]", ctx => {
                        Thread.Sleep(2000);
                    });
        }

        private static void SetupDiscovery() {
            DiscoveryProxy.DiscoveryProxy proxy = new DiscoveryProxy.DiscoveryProxy(new Uri("net.tcp://localhost:8001/Probe"));
            proxy.SetupProxy();
        }

        private static void LoadPlugins() {
            // PluginService pluginService = new PluginService(@"C:\Users\hoffmann\Documents\FH Flensburg\7. Semester\Verteilte Systeme\VS-Hausarbeit\DemoPlugin\bin\Debug");
            PluginService.GetInstance().Startup();
        }

        private static void OnExit() {
            PluginService.GetInstance().OnStop();
            GlobalState.GetInstance().ServiceProxy.SayGoodbye(
                GlobalState.GetInstance().ServerId
            );
        }
    }
}
