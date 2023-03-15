using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.ServiceContracts;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.ConcreatService.ClientDiscovery")]
    public interface IClientDiscoveryService {
        [OperationContract]
        void Connect(string clientId);

        [OperationContract]
        Dictionary<string, ServerStatus> GetServerStatus();
    }

    public class ClientDiscoveryService : IClientDiscoveryService {
        private readonly ILogger logger = Logger.Instance;
        public void Connect(string clientId) {
            this.logger.Info($"Client {clientId} connected to system");
        }
        public Dictionary<string, ServerStatus> GetServerStatus() {
            this.logger.Debug("Get status from workers...");
            var response = new Dictionary<string, ServerStatus>();
            foreach (var worker in ServiceState.GetInstance().Workers) {
                var status = worker.Value.ReportStatus();
                response.Add(worker.Key, status);
            }
            return response;
        }
    }

    public interface IClientDiscoveryServiceCallback {
        
    }
}
