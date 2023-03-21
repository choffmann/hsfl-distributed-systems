using System.Collections.Generic;
using System.ServiceModel;
using hsfl.ceho5518.vs.LoggerService;

namespace hsfl.ceho5518.vs.server.ServiceContracts.ServerDiscoveryService {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.ConcreatService.ServerDiscovery",
        CallbackContract = typeof(IServerDiscoveryServiceCallback))]
    public interface IServerDiscoveryService {
        [OperationContract]
        void Connect(string workerId);

        [OperationContract(IsOneWay = true)]
        void Broadcast(string message);

        [OperationContract]
        void GetStatus();

        [OperationContract(IsOneWay = true)]
        void SayGoodbye(string workerId);
    }

    public class ServerDiscoveryService : IServerDiscoveryService {
        private readonly ILogger logger = Logger.Instance;
        private readonly Dictionary<string, IServerDiscoveryServiceCallback> workers = ServiceState.GetInstance().Workers;

        public void Connect(string workerId) {
            this.logger.Info($"New Worker with id {workerId} connected to the system");
            var callback = OperationContext.Current.GetCallbackChannel<IServerDiscoveryServiceCallback>();
            if (!this.workers.ContainsValue(callback)) {
                this.logger.Debug($"Save Worker with id {workerId} in dictionary.");
                this.workers.Add(workerId, callback);
            }
        }

        public void Broadcast(string message) {
            foreach (var worker in this.workers) {
                worker.Value.OnMessage(message);
                this.logger.Info($"Send message to Worker with id {worker.Key}");
            }
        }
        public void GetStatus() {
            foreach (var worker in this.workers) {
                var status = worker.Value.ReportStatus();
                this.logger.Debug($"Status from worker: {worker.Key}");
                this.logger.Debug($"-> {status}");
            }
        }
        public void SayGoodbye(string workerId) {
            bool removeSuccess = this.workers.Remove(workerId);
            if (removeSuccess) {
                this.logger.Info($"Worker {workerId} logged out :waving_hand:");
            }
        }
    }

    public interface IServerDiscoveryServiceCallback {
        [OperationContract(IsOneWay = true)]
        void OnMessage(string message);

        [OperationContract]
        ServerStatus ReportStatus();
    }
}
