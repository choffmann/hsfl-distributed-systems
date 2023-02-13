using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.ConcreatService.ServerDiscovery")]
    public interface IServerDiscoveryService {
        [OperationContract]
        void Connect(string message);
    }

    public class ServerDiscoveryService : IServerDiscoveryService {
        private ILogger logger = Logger.Instance;
        public void Connect(string workerId) {
            logger.Info($"New Worker with id {workerId} connected to the system");
            logger.Debug($"Worker adresse: xxx");
            logger.Debug($"Worker mac: xxx");
            logger.Debug($"Worker information: xxx");
        }
    }
}
