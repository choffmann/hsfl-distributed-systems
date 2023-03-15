using System.Collections.Generic;
using System.Runtime.CompilerServices;
using hsfl.ceho5518.vs.server.ConcreatService;

namespace hsfl.ceho5518.vs.ServiceContracts {
    public class ServiceState {
        static ServiceState instance;
        public Dictionary<string, IServerDiscoveryServiceCallback> Workers { get; set; } = new Dictionary<string, IServerDiscoveryServiceCallback>();
        private ServiceState() {
        }
        
        public static ServiceState GetInstance() {
            return instance ?? (instance = new ServiceState());
        }
    }
}
