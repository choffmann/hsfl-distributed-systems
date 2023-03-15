using System.Collections.Generic;
using System.ServiceModel;
using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.Client.State;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.ConcreatService;
using hsfl.ceho5518.vs.ServiceContracts;

namespace hsfl.ceho5518.vs.Client {
    public interface IInvokeClientDiscovery {
        void Setup(EndpointAddress endpointAddress);
        void Connect();
        Dictionary<string, ServerStatus> GetServerStatus();
    }
    
    public class InvokeClientDiscovery : IInvokeClientDiscovery, IClientDiscoveryServiceCallback {
        private readonly ILogger logger = Logger.Instance;
        private IClientDiscoveryService serviceProxy;

        //private DuplexChannelFactory<IClientDiscoveryService> channelFactory;
        private ChannelFactory<IClientDiscoveryService> channelFactory;

        public void Setup(EndpointAddress endpointAddress) {
            this.channelFactory = new ChannelFactory<IClientDiscoveryService>(new NetTcpBinding(), endpointAddress);
            this.serviceProxy = this.channelFactory.CreateChannel();
        }
        
        public void Connect() {
            this.serviceProxy.Connect("123-456-789");
        }
        public Dictionary<string, ServerStatus> GetServerStatus() {
            return this.serviceProxy.GetServerStatus();
        }
    }
}
