using hsfl.ceho5518.vs.server.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.ServiceContracts;

namespace hsfl.ceho5518.vs.server.Sate {
    public class GlobalState {
        static GlobalState instance;
        public ServerState ServerState { get; set; }
        public Guid ServerId { get; } = Guid.NewGuid();
        public string ApplicationDir { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "hsfl",
            "ceho5518", "distributed-systems");
        public ConcreatService.IServerDiscoveryService ServiceProxy { get; set; }
        public ServerStatus ServerStatus { get; set; } = ServerStatus.STARTING;
        // Debug things...
        public bool ClearAllOnStart { get; set; } = false;

        private GlobalState() {
            // Set state to WORKER on default
            ServerState = ServerState.WORKER;
        }

        public static GlobalState GetInstance() {
            return instance ??= new GlobalState();
        }
    }
}
