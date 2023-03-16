using System;
using System.ComponentModel;
using hsfl.ceho5518.vs.Client.Logger;
using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.LoggerService;
using Spectre.Console;
using Spectre.Console.Cli;

namespace hsfl.ceho5518.vs.Client.Commands {
    public sealed class StatusCommand : Command<StatusCommand.Settings> {
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly IConnector _connector;
        private ILogger logger;

        public sealed class Settings : DefaultSettings {
        }

        public StatusCommand(IConnector connector, IInvokeClientDiscovery invokeClient) {
            this._connector = connector ?? throw new ArgumentNullException(nameof(connector));
            this._invokeClient = invokeClient ?? throw new ArgumentNullException(nameof(invokeClient));
            this.logger = ClientLogger.Logger;
        }

        public override int Execute(CommandContext context, Settings settings) {
            this._connector.Setup();
            PrintStatusTable();

            return 0;
        }
        
        private void PrintStatusTable() {
            var status = this._invokeClient.GetServerStatus();
            var table = new Table();
            table.AddColumn("Role");
            table.AddColumn("Status");
            table.AddColumn("Server ID");

            foreach (var server in status) {
                string worker = server.IsWorker ? "WORKER" : "MASTER";
                table.AddRow(worker, server.CurrentState.ToString(), server.Id);
            }

            AnsiConsole.Write(table);
        }
    }
}
