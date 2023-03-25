using System.Collections.Generic;
using System.ServiceModel;
using hsfl.ceho5518.vs.server.ServiceContracts.ServerDiscoveryService;

namespace hsfl.ceho5518.vs.server.ServiceContracts {
    public class ServiceState {
        static readonly ServiceState instance = new ServiceState();
        public Dictionary<string, IServerDiscoveryServiceCallback> Workers { get; set; } =
            new Dictionary<string, IServerDiscoveryServiceCallback>();
        static ServiceState() { }
        private ServiceState() {
            //ServerService = new ServiceContracts.ServerDiscoveryService.ServerDiscoveryService();
        }
        public static ServiceState Instance { get { return instance; } }
    }
}
