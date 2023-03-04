using hsfl.ceho5518.vs.server.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.Sate {
    public class GlobalState {
        static GlobalState instance;
        public ServerState ServerState { get; set; }
        public Guid ServerId { get; } = Guid.NewGuid();
        public string ApplicationDir { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "hsfl", "ceho5518", "distributed-systems");

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
