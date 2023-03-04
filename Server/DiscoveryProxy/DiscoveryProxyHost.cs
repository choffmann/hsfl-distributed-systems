using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    public class DiscoveryProxyHost {
        private readonly ILogger logger = Logger.Instance;
        private readonly Uri probeEndpointAddress;
        private readonly Uri announcementEndpointAddress;
        private readonly ServiceHost proxyServiceHost;

        public DiscoveryProxyHost() {
            this.probeEndpointAddress = new Uri($"net.tcp://{Environment.MachineName}:8001/Probe");
            this.announcementEndpointAddress = new Uri($"net.tcp://{Environment.MachineName}:9021/Announcement");
            this.proxyServiceHost = new ServiceHost(new DiscoveryProxyService());
        }

        public void Start() {
            this.logger.Info("Starting Discovery Proxy...");
            try {
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                var discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(this.probeEndpointAddress)) {
                    IsSystemEndpoint = false
                };
                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                var announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(), new EndpointAddress(this.announcementEndpointAddress));

                this.proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);
                this.proxyServiceHost.AddServiceEndpoint(announcementEndpoint);
                this.proxyServiceHost.Open();
                this.logger.Info("Proxy Service started.");
            } catch (CommunicationException e) {
                this.logger.Exception(e);
            } catch (TimeoutException e) {
                this.logger.Exception(e);
            }
        }

        public void Stop() {
            if (this.proxyServiceHost.State == CommunicationState.Closed)
                return;
            this.logger.Info("Aborting the Proxy service...");
            this.proxyServiceHost.Abort();
        }
    }
}
