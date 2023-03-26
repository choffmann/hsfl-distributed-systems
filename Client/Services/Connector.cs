using System;
using hsfl.ceho5518.vs.Client.Logger;
using hsfl.ceho5518.vs.LoggerService;
using Spectre.Console;

namespace hsfl.ceho5518.vs.Client.Services {

    public interface IConnector {
        void Setup();
    }
    
    public class Connector : IConnector {
        private readonly IDiscoveryMaster _discovery;
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly ILogger logger;
        
        public Connector(IDiscoveryMaster discovery, IInvokeClientDiscovery invoker) {
            this._discovery = discovery ?? throw new ArgumentNullException(nameof(discovery));
            this._invokeClient = invoker ?? throw new ArgumentNullException(nameof(invoker));
            this.logger = ClientLogger.Logger;
        }
        
        public void Setup() {
            AnsiConsole.Status().Start("Versuche mit Master im Netzwerk zu verbinden...", ctx => {
                this.logger.Info("Setzte Verbindung zum Master");
                this._discovery.ProbeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
                var endpointAddress = this._discovery.SetupProxy();
                this._invokeClient.Setup(endpointAddress);
                this.logger.Info("Verbinde zum Master");
                this._invokeClient.Connect(Guid.NewGuid().ToString());
            });
            this.logger.Success("Verbindung zum Master wurde erfolgriech aufgebaut.");
        }
    }
}
