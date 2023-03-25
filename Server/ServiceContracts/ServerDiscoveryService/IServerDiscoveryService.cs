using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Plugins;
using PluginContract;
using Logger = hsfl.ceho5518.vs.LoggerService.Logger;

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

        [OperationContract]
        int ExecutePlugin(string pluginName, string input);

        [OperationContract(IsOneWay = true)]
        void SayGoodbye(string workerId);

        [OperationContract]
        int RegisterNewPlugin(byte[] plugin);
    }

    public class ServerDiscoveryService : IServerDiscoveryService {
        private readonly ILogger logger = Logger.Instance;
        private readonly Dictionary<string, IServerDiscoveryServiceCallback> workers = ServiceState.Instance.Workers;

        public ServerDiscoveryService() { }

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
        public int ExecutePlugin(string pluginName, string input) {
            foreach (var worker in this.workers.Where(worker => worker.Value.ReportStatus() == ServerStatus.IDLE)) {
                try {
                    // Check if Worker has plugin installed
                    var loadedPlugins = worker.Value.OnPluginRequest();
                    foreach (byte[] pluginBytes in from plugin in PluginService.GetInstance().PluginsList
                             where !loadedPlugins.Contains(plugin.Name)
                             select File.ReadAllBytes(Path.Combine(PluginService.GetInstance().PluginPath, pluginName + ".dll"))) {
                        this.logger.Info($"Plugin ist nicht auf dem Worker installiert und wird an Worker übertragen");
                        worker.Value.OnRegisterNewPlugin(pluginBytes);
                    }

                    // Run Plugin
                    return worker.Value.OnRunPlugin(pluginName, input);
                }
                catch (Exception e) {
                    this.logger.Exception(e);
                }
            }

            this.logger.Warning("Aktuell sind keine Worker verfügbar. Breche Operation ab.");
            return 10;
        }
        public void SayGoodbye(string workerId) {
            bool removeSuccess = this.workers.Remove(workerId);
            if (removeSuccess) {
                this.logger.Info($"Worker {workerId} logged out :waving_hand:");
            }
        }
        public int RegisterNewPlugin(byte[] plugin) {
            int response = this.workers.Sum(worker => worker.Value.OnRegisterNewPlugin(plugin));
            return response == 0 ? 0 : 28;
        }
    }

    public interface IServerDiscoveryServiceCallback {
        [OperationContract(IsOneWay = true)]
        void OnMessage(string message);

        [OperationContract]
        List<string> OnPluginRequest();

        [OperationContract]
        ServerStatus ReportStatus();

        [OperationContract]
        int OnRegisterNewPlugin(byte[] plugin);

        [OperationContract]
        int OnRunPlugin(string pluginName, string input);
    }
}
