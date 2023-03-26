using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using hsfl.ceho5518.vs.Client.Logger;
using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Plugins;
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
            return 0;
        }
    }

    public sealed class PluginUploadCommand : Command<PluginUploadCommand.Settings>, ICommandLimiter<PluginCommand.Settings> {
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly IConnector _connector;
        private ILogger logger;
        public PluginUploadCommand(IConnector connector, IInvokeClientDiscovery invokeClient) {
            this._connector = connector ?? throw new ArgumentNullException(nameof(connector));
            this._invokeClient = invokeClient ?? throw new ArgumentNullException(nameof(invokeClient));
            this.logger = ClientLogger.Logger;
        }

        public sealed class Settings : CommandSettings {
            [CommandArgument(0, "<PATH>")]
            public string PluginPath { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings) {
            this._connector.Setup();
            byte[] assembly = File.ReadAllBytes(settings.PluginPath);
            return this._invokeClient.UploadPlugin(assembly);
        }
    }

    public sealed class PluginRunCommand : Command<PluginRunCommand.Settings>, ICommandLimiter<PluginCommand.Settings> {
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly IConnector _connector;
        private ILogger logger;

        public PluginRunCommand(IConnector connector, IInvokeClientDiscovery invokeClient) {
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
            table.AddColumn("CommandName");
            table.AddColumn("Active");
            table.AddColumn("Size");

            foreach (var plugin in status) {
                table.AddRow(plugin.Name, plugin.CommandName, plugin.Activated.ToString(), plugin.Size.ToString());
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"Insgesamt sind {status.Count} Plugins verfügbar");
        }
    }

    public sealed class PluginListCommand : Command<PluginListCommand.Settings>, ICommandLimiter<PluginCommand.Settings> {
        public sealed class Settings : CommandSettings {
            [CommandArgument(0, "<PluginName>")]
            public string PluginName { get; set; }
            
            [CommandArgument(1, "[InputArgs]")]
            public string[] Number { get; set; }
        }

        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly IConnector _connector;
        private ILogger logger;

        public PluginListCommand(IConnector connector, IInvokeClientDiscovery invokeClient) {
            this._connector = connector ?? throw new ArgumentNullException(nameof(connector));
            this._invokeClient = invokeClient ?? throw new ArgumentNullException(nameof(invokeClient));
            this.logger = ClientLogger.Logger;
        }

        public override int Execute(CommandContext context, Settings settings) {
            this._connector.Setup();
            var response = this._invokeClient.ExecutePlugin(settings.PluginName, settings.Number);
            AnsiConsole.MarkupLine(ConvertReturnCode.Convert(response));
            return response;
        }
    }
}
