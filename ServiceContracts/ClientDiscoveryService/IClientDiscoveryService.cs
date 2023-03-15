using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
        List<ServerStatusDetail> GetServerStatus();
    }

    public class ClientDiscoveryService : IClientDiscoveryService {
        private readonly ILogger logger = Logger.Instance;
        public void Connect(string clientId) {
            this.logger.Info($"Client {clientId} connected to system");
        }
        public List<ServerStatusDetail> GetServerStatus() {
            var response = new List<ServerStatusDetail>();
            var masterStatus = new ServerStatusDetail {
                CurrentState = ServiceState.GetInstance().CurrentState,
                Id = ServiceState.GetInstance().CurrentId,
                IsWorker = false
            };

            response.Add(masterStatus);
            
            foreach (var worker in ServiceState.GetInstance().Workers) {
                try {
                    var status = worker.Value.ReportStatus();
                    var workerStatus = new ServerStatusDetail {
                        CurrentState = status,
                        Id = worker.Key,
                        IsWorker = true
                    };
                    response.Add(workerStatus);
                }
                catch (Exception e) {
                    this.logger.Warning($"Error while calling worker {worker.Key}");
                    var workerStatus = new ServerStatusDetail {
                        CurrentState = ServerStatus.ERROR,
                        Id = worker.Key,
                        IsWorker = true
                    };
                    response.Add(workerStatus);
                }
            }
            return response;
        }
    }

    public interface IClientDiscoveryServiceCallback { }
}
