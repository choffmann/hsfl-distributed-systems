using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using hsfl.ceho5518.vs.Client.Logger;
using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.LoggerService;
using Spectre.Console;
using Spectre.Console.Cli;

namespace hsfl.ceho5518.vs.Client.Commands {
    public sealed class PluginCommand : Command<PluginCommand.Settings> {
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly IConnector _connector;
        private ILogger logger;

        public sealed class Settings : CommandSettings { }

        public PluginCommand(IConnector connector, IInvokeClientDiscovery invokeClient) {
            this._connector = connector ?? throw new ArgumentNullException(nameof(connector));
            this._invokeClient = invokeClient ?? throw new ArgumentNullException(nameof(invokeClient));
            this.logger = ClientLogger.Logger;
        }

        public override int Execute(CommandContext context, Settings settings) {
            this._connector.Setup();


            byte[] assembly = File.ReadAllBytes(
                @"C:\Users\hoffmann\Documents\FH Flensburg\7. Semester\Verteilte Systeme\VS-Hausarbeit\DemoPlugin\bin\Debug\DemoPlugin.dll");
            this._invokeClient.UploadPlugin(assembly);
            return 0;
        }
    }

    public sealed class PluginStatusCommand : Command<PluginStatusCommand.Settings>, ICommandLimiter<PluginCommand.Settings> {
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly IConnector _connector;
        private ILogger logger;

        public PluginStatusCommand(IConnector connector, IInvokeClientDiscovery invokeClient) {
            this._connector = connector ?? throw new ArgumentNullException(nameof(connector));
            this._invokeClient = invokeClient ?? throw new ArgumentNullException(nameof(invokeClient));
            this.logger = ClientLogger.Logger;
        }

        public sealed class Settings : CommandSettings { }

        public override int Execute(CommandContext context, Settings settings) {
            this._connector.Setup();
            PrintStatusTable();
            
            return 0;
        }
        
        private void PrintStatusTable() {
            var status = this._invokeClient.PluginStatus().Plugins;
            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Active");
            table.AddColumn("Size");

            foreach (var plugin in status) {
                table.AddRow(plugin.Name, plugin.Activated.ToString(), plugin.Size.ToString());
            }

            AnsiConsole.Write(table);
        }
    }
}
