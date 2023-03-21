using System.Collections.Generic;
using hsfl.ceho5518.vs.server.ServiceContracts.ServerDiscoveryService;

namespace hsfl.ceho5518.vs.server.ServiceContracts {
    public class ServiceState {
        static ServiceState instance;
        public Dictionary<string, IServerDiscoveryServiceCallback> Workers { get; set; } =
            new Dictionary<string, IServerDiscoveryServiceCallback>();
        public ServerStatus CurrentState { get; set; }
        public string CurrentId { get; set; }
        private ServiceState() { }

        public static ServiceState GetInstance() {
            return instance ?? (instance = new ServiceState());
        }
    }
}
