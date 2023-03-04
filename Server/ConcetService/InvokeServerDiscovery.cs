using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Sate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    public class InvokeServerDiscovery : IServerDiscoveryServiceCallback {
        private readonly ILogger logger = Logger.Instance;
        private IServerDiscoveryService serviceProxy;

        public void Connect(EndpointAddress endpointAddress) {
            this.logger.Info($"Invoking ServerDiscoveryServiceClient at {endpointAddress.Uri}");
            var instanceContext = new InstanceContext(this);
            var channelFactory = new DuplexChannelFactory<IServerDiscoveryService>(instanceContext, new NetTcpBinding(), endpointAddress);
            this.serviceProxy = channelFactory.CreateChannel();
            this.serviceProxy.Connect(GlobalState.GetInstance().ServerId.ToString());
        }

        public void OnMessage(string message) {
            this.logger.Info($"Message from Master: {message}");
        }
    }
}
