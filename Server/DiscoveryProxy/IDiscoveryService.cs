using hsfl.ceho5518.vs.server.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.DiscoveryProxy")]
    public interface IDiscoveryService {
        [OperationContract]
        void Connect(string message);
    }

    public class DiscoveryService : IDiscoveryService {
        public void Connect(string workerId) {
            Logger.Info($"New Worker with id {workerId} connected to the System.");
            Logger.Debug($"Worker adresse:     xxx");
            Logger.Debug($"Worker mac:         xxx");
            Logger.Debug($"Worker information: xxx");
        }
    }
}
