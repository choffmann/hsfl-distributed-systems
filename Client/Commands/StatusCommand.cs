using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.Client.State;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.ConcreatService;
using Spectre.Console.Cli;

namespace hsfl.ceho5518.vs.Client.Commands {
    public sealed class StatusCommand : Command<StatusCommand.Settings> {
        private readonly IDiscoveryMaster _discovery;
        private readonly IInvokeClientDiscovery _invokeClient;
        public sealed class Settings : CommandSettings {
            [Description("Enable verbose mode")]
            [CommandOption("--verbose")]
            [DefaultValue(false)]
            public bool VerboseMode { get; set; }
        }

        public StatusCommand(IDiscoveryMaster discovery, IInvokeClientDiscovery invokeClient) {
            this._discovery = discovery ?? throw new ArgumentNullException(nameof(discovery));
            this._invokeClient = invokeClient ?? throw new ArgumentNullException(nameof(invokeClient));
        }

        public override int Execute(CommandContext context, Settings settings) {
            ILogger logger;
            if (settings.VerboseMode) logger = Logger.Instance;
            else logger = new NoLogger();

            Setup();
            GetStatus();
            
            return 0;
        }


        private void Setup() {
            this._discovery.ProbeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
            var endpointAddress = this._discovery.SetupProxy();
            this._invokeClient.Setup(endpointAddress);
            this._invokeClient.Connect();
        }
        
        private void GetStatus() {
            var status = this._invokeClient.GetServerStatus();
            foreach (var value in status) {
                Logger.Instance.Info($"{value.Key} -> {value.Value}");
            }
        }
    }
}
