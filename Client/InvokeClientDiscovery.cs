using System.ServiceModel;
using hsfl.ceho5518.vs.Client.State;
using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.ConcreatService;

namespace hsfl.ceho5518.vs.Client {
    public class InvokeClientDiscovery : IClientDiscoveryServiceCallback {
        private readonly ILogger logger = Logger.Instance;

        //private DuplexChannelFactory<IClientDiscoveryService> channelFactory;
        private ChannelFactory<IClientDiscoveryService> channelFactory;

        public void Connect(EndpointAddress endpointAddress) {
            this.logger.Debug($"Invoking ServerDiscoveryServiceClient at {endpointAddress.Uri}");
            //var instanceContext = new InstanceContext(this);
            this.channelFactory = new ChannelFactory<IClientDiscoveryService>(new NetTcpBinding(), endpointAddress);
            ClientState.GetInstance().ServiceProxy = this.channelFactory.CreateChannel();
            ClientState.GetInstance().ServiceProxy.Connect("123-456-789");
        }
    }
}
