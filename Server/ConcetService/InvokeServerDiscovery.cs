using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Sate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.ServiceContracts;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    public class InvokeServerDiscovery : IServerDiscoveryServiceCallback {
        private readonly ILogger logger = Logger.Instance;
        private DuplexChannelFactory<IServerDiscoveryService> channelFactory;

        public void Connect(EndpointAddress endpointAddress) {
            this.logger.Info($"Invoking ServerDiscoveryServiceClient at {endpointAddress.Uri}");
            var instanceContext = new InstanceContext(this);
            this.channelFactory = new DuplexChannelFactory<IServerDiscoveryService>(instanceContext, new NetTcpBinding(), endpointAddress);
            GlobalState.GetInstance().ServiceProxy = this.channelFactory.CreateChannel();
            GlobalState.GetInstance().ServiceProxy.Connect(GlobalState.GetInstance().ServerId.ToString());
        }

        public void OnMessage(string message) {
            this.logger.Info($"Message from Master: {message}");
        }
        public ServerStatus ReportStatus() {
            return GlobalState.GetInstance().ServerStatus;
        }
    }
}
