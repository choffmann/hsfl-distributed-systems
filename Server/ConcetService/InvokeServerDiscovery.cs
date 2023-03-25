using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Sate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.server.Plugins;
using hsfl.ceho5518.vs.server.ServiceContracts;
using hsfl.ceho5518.vs.server.ServiceContracts.ServerDiscoveryService;
using PluginContract;
using Logger = hsfl.ceho5518.vs.LoggerService.Logger;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    public class InvokeServerDiscovery : IServerDiscoveryServiceCallback {
        private readonly ILogger logger = Logger.Instance;
        private DuplexChannelFactory<ServiceContracts.ServerDiscoveryService.IServerDiscoveryService> channelFactory;

        public void Connect(EndpointAddress endpointAddress) {
            this.logger.Info($"Invoking ServerDiscoveryServiceClient at {endpointAddress.Uri}");
            var instanceContext = new InstanceContext(this);
            this.channelFactory =
                new DuplexChannelFactory<ServiceContracts.ServerDiscoveryService.IServerDiscoveryService>(instanceContext, new NetTcpBinding(),
                    endpointAddress);
            GlobalState.GetInstance().ServiceProxy = this.channelFactory.CreateChannel();
            GlobalState.GetInstance().ServiceProxy.Connect(GlobalState.GetInstance().ServerId.ToString());
        }

        public void OnMessage(string message) {
            this.logger.Info($"Message from Master: {message}");
        }
        public List<string> OnPluginRequest() {
            return PluginService.GetInstance().PluginsList.Select(x => x.Name).ToList();
        }
        public ServerStatus ReportStatus() {
            return GlobalState.GetInstance().ServerStatus;
        }
        public int OnRegisterNewPlugin(byte[] plugin) {
            try {
                PluginService.GetInstance().OnPluginUpload(plugin);
                return 0;
            }
            catch (Exception e) {
                this.logger.Error($"Ein unerwarteter Fehler wurde beim registrieren einses Plugins.");
                this.logger.Exception(e);
                return 28;
            }
        }
        public int OnRunPlugin(string pluginName, string input) {
            try {
                
                this.logger.Info("Register new Plugin");
                foreach (var plugin in PluginService.GetInstance().PluginsList.Where(plugin => plugin.Name.Equals(pluginName))) {
                    plugin.OnServerExecute(input);
                }
                return 0;
            }
            catch (Exception e) {
                this.logger.Error($"Ein unerwarteter Fehler wurde beim ausführen des Plugins {pluginName} ausgelöst.");
                this.logger.Exception(e);
                return 29;
            }
        }
    }
}
