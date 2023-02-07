using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.ConcreatService.ClientDiscovery")]
    internal interface IClientDiscoveryService {
        [OperationContract]
        void Connect(string message);
    }

    public class ClientDiscoveryService : IClientDiscoveryService {
        public void Connect(string message) {
            Logger.Info($"Client with id {message} connected to the system");
        }
    }
}
