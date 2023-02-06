using hsfl.ceho5518.vs.server.plugins;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server {
    internal class Program {
        static void Main(string[] args) {
            AnsiConsole.MarkupLine("Starting Server...");
            string pluginPath = AnsiConsole.Ask<string>("Pfad zum Plugin");
            // PluginService pluginService = new PluginService("C:\\Users\\hoffmann\\Documents\\FH Flensburg\\7. Semester\\Verteilte Systeme\\VS-Hausarbeit\\DemoPlugin\\bin\\Debug");
            PluginService pluginService = new PluginService(pluginPath);

            Console.WriteLine("Press to reload plugins");
            Console.ReadLine();

            pluginService.ReloadPlugins();

            Console.ReadLine();

        }
    }
}
