using System;
using hsfl.ceho5518.vs.server.ConcreatService;

namespace hsfl.ceho5518.vs.Client.State {
    public class ClientState {
        static ClientState instance;
        public IClientDiscoveryService ServiceProxy { get; set; }
        public Guid ClientId { get; } = Guid.NewGuid();

        private ClientState() {
        }
        
        public static ClientState GetInstance() {
            return instance ?? (instance = new ClientState());
        }
    }
}
