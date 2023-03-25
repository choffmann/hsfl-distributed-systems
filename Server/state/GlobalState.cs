using hsfl.ceho5518.vs.server.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.server.ConcreatService;
using hsfl.ceho5518.vs.server.ServiceContracts;

namespace hsfl.ceho5518.vs.server.Sate {
    public class GlobalState {
        static GlobalState instance;
        private ServerStatus _serverStatus = ServerStatus.STARTING;
        public ServerState ServerState { get; set; }
        public string ServerId { get; } = Guid.NewGuid().ToString();
        public string ApplicationDir { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "hsfl",
            "ceho5518", "distributed-systems");
        public ServiceContracts.ServerDiscoveryService.IServerDiscoveryService ServiceProxy { get; set; }

        public ServerStatus ServerStatus {
            get {
                return this._serverStatus;
            }
            set {
                ServiceContracts.ServiceState.Instance.CurrentState = value;
                this._serverStatus = value;
            }
        }

        // Debug things...
        public bool ClearAllOnStart { get; set; } = false;

        private GlobalState() {
            // Set state to WORKER on default
            ServerState = ServerState.WORKER;
            ServiceState.Instance.CurrentId = ServerId.ToString();
        }

        public static GlobalState GetInstance() {
            return instance ??= new GlobalState();
        }
    }
}
