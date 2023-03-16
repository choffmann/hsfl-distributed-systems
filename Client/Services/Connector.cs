using System;
using hsfl.ceho5518.vs.LoggerService;

namespace hsfl.ceho5518.vs.Client.Services {

    public interface IConnector {
        void Setup();
    }
    
    public class Connector : IConnector {
        private readonly IDiscoveryMaster _discovery;
        private readonly IInvokeClientDiscovery _invokeClient;
        private readonly ILogger logger;
        
        public Connector(IDiscoveryMaster discovery, IInvokeClientDiscovery invoker, ILogger logger) {
            this._discovery = discovery ?? throw new ArgumentNullException(nameof(discovery));
            this._invokeClient = invoker ?? throw new ArgumentNullException(nameof(invoker));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));;
        }
        
        public void Setup() {
            this.logger.Info("Setup Connection to Master");
            this._discovery.ProbeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
            var endpointAddress = this._discovery.SetupProxy();
            this._invokeClient.Setup(endpointAddress);
            this.logger.Info("Connection to Master");
            this._invokeClient.Connect();
            
            this.logger.Success("Connection to Master successful");
        }
    }
}
