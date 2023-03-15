using System;
using hsfl.ceho5518.vs.server.ConcreatService;

namespace hsfl.ceho5518.vs.Client.State {
    public class GlobalState {
        static GlobalState instance;
        public IClientDiscoveryService ServiceProxy { get; set; }
        public Guid ClientId { get; } = Guid.NewGuid();

        private GlobalState() {
        }
        
        public static GlobalState GetInstance() {
            if (instance == null) {
                return new GlobalState();
            } else {
                return instance;
            }
        }
    }
}
