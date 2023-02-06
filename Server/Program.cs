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
            PluginService pluginService = new PluginService("C:\\Users\\hoffmann\\Documents\\FH Flensburg\\7. Semester\\Verteilte Systeme\\VS-Hausarbeit\\DemoPlugin\\bin\\Debug");

            Console.WriteLine("Press to reload plugins");
            Console.ReadLine();

            pluginService.ReloadPlugins();

            Console.ReadLine();

        }
    }
}
