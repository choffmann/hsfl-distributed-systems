using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.ConcreatService;
using hsfl.ceho5518.vs.server.ServiceContracts;
using hsfl.ceho5518.vs.server.ServiceContracts.ClientDiscoveryService;
using hsfl.ceho5518.vs.server.ServiceContracts.Model;

namespace hsfl.ceho5518.vs.Client {
    public interface IInvokeClientDiscovery: IClientDiscoveryService {
        void Setup(EndpointAddress endpointAddress);
    }
    
    public class InvokeClientDiscovery : IInvokeClientDiscovery, IClientDiscoveryServiceCallback {
        private IClientDiscoveryService serviceProxy;

        //private DuplexChannelFactory<IClientDiscoveryService> channelFactory;
        private ChannelFactory<IClientDiscoveryService> channelFactory;

        public void Setup(EndpointAddress endpointAddress) {
            this.channelFactory = new ChannelFactory<IClientDiscoveryService>(new NetTcpBinding(), endpointAddress);
            this.serviceProxy = this.channelFactory.CreateChannel();
        }
        
        public void Connect(string clientId) {
            this.serviceProxy.Connect(clientId);
        }
        public List<ServerStatusDetail> GetServerStatus() {
            return this.serviceProxy.GetServerStatus();
        }
        public int UploadPlugin(byte[] assembly) {
            return this.serviceProxy.UploadPlugin(assembly);
        }
        public PluginStatus PluginStatus() {
            return this.serviceProxy.PluginStatus();
        }
        public int ExecutePlugin(string pluginCommand, string value) {
            return this.serviceProxy.ExecutePlugin(pluginCommand, value);
        }
    }
}
