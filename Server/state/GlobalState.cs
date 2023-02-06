using hsfl.ceho5518.vs.server.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.sate {
    public class GlobalState {
        static GlobalState instance;
        public ServerState serverState { get; set; }
        
        protected GlobalState() {
            // Set state to WORKER on default
            serverState = ServerState.WORKER;
        }

        public static GlobalState getInstance() {
            if (instance == null) {
                instance = new GlobalState();
            }
            return instance;
        }
    }
}
