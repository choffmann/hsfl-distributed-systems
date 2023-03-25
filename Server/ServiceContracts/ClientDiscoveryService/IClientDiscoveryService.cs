using System;
using System.Collections.Generic;
using System.ServiceModel;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Plugins;
using hsfl.ceho5518.vs.server.Sate;
using hsfl.ceho5518.vs.server.ServiceContracts.Model;

namespace hsfl.ceho5518.vs.server.ServiceContracts.ClientDiscoveryService {
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.ConcreatService.ClientDiscovery")]
    public interface IClientDiscoveryService {
        [OperationContract]
        void Connect(string clientId);

        [OperationContract]
        List<ServerStatusDetail> GetServerStatus();

        [OperationContract]
        int UploadPlugin(byte[] assembly);

        [OperationContract]
        PluginStatus PluginStatus();

        [OperationContract]
        int ExecutePlugin(string pluginCommand, string[] args);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ClientDiscoveryService : IClientDiscoveryService {
        private readonly ILogger logger = Logger.Instance;
        public void Connect(string clientId) {
            this.logger.Debug($"Client {clientId} connected to system");
        }
        public List<ServerStatusDetail> GetServerStatus() {
            var response = new List<ServerStatusDetail>();
            var masterStatus = new ServerStatusDetail {
                CurrentState = GlobalState.GetInstance().ServerStatus,
                Id = GlobalState.GetInstance().ServerId,
                IsWorker = false
            };

            response.Add(masterStatus);

            foreach (var worker in ServiceState.Instance.Workers) {
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
        public int UploadPlugin(byte[] assembly) {
            PluginService.GetInstance().OnPluginUpload(assembly);
            var serverService = new ServerDiscoveryService.ServerDiscoveryService();
            return serverService.RegisterNewPlugin(assembly);
        }
        public PluginStatus PluginStatus() {
            return PluginService.GetInstance().ReportPlugins();
        }
        public int ExecutePlugin(string pluginCommand, string[] args) {
            var pluginList = PluginService.GetInstance().PluginsList;
            foreach (var plugin in pluginList) {
                if (plugin.CommandName.Equals(pluginCommand)) {
                    var serverService = new ServerDiscoveryService.ServerDiscoveryService();
                    return serverService.ExecutePlugin(plugin.Name, args);
                }
            }
            return 20;
        }
    }

    public interface IClientDiscoveryServiceCallback { }
}
