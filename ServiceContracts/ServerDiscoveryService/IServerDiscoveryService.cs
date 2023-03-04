using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.ConcreatService.ServerDiscovery", CallbackContract = typeof(IServerDiscoveryServiceCallback))]
    public interface IServerDiscoveryService {
        [OperationContract]
        void Connect(string message);

        [OperationContract(IsOneWay = true)]
        void Broadcast(string message);
    }

    public class ServerDiscoveryService : IServerDiscoveryService {
        private readonly ILogger logger = Logger.Instance;
        //private List<IServerDiscoveryServiceCallback> workers = new List<IServerDiscoveryServiceCallback>();
        private readonly Dictionary<string, IServerDiscoveryServiceCallback> workers = new Dictionary<string, IServerDiscoveryServiceCallback>();
        public void Connect(string workerId) {
            this.logger.Info($"New Worker with id {workerId} connected to the system");
            var callback = OperationContext.Current.GetCallbackChannel<IServerDiscoveryServiceCallback>();
            if (!this.workers.ContainsValue(callback)) {
                this.logger.Debug($"Save Worker with id {workerId} in dictionary.");
                this.workers.Add(workerId, callback);
            }

            if (this.workers.Count == 1) {
                Timer timer = new Timer();
                timer.Interval = 5000;
                timer.Elapsed += (sender, args) => {
                    Broadcast($"Neuer Aufruf: {DateTime.Now}");
                };
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            
        }
        
        public void Broadcast(string message) {
            foreach (var worker in this.workers) {
                worker.Value.OnMessage(message);
                this.logger.Info($"Send message to Worker with id {worker.Key}");
            }
        }
    }

    public interface IServerDiscoveryServiceCallback {
        [OperationContract(IsOneWay = true)]
        void OnMessage(string message);
    }
}
