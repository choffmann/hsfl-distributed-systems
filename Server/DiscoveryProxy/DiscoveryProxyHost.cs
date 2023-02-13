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
        private ILogger logger = Logger.Instance;
        private Uri probeEndpointAddress;
        private Uri announcementEndpointAddress;
        private ServiceHost proxyServiceHost;

        public DiscoveryProxyHost() {
            this.probeEndpointAddress = new Uri($"net.tcp://{Environment.MachineName}:8001/Probe");
            this.announcementEndpointAddress = new Uri($"net.tcp://{Environment.MachineName}:9021/Announcement");
            this.proxyServiceHost = new ServiceHost(new DiscoveryProxyService());
        }

        public void Start() {
            logger.Info("Starting Discovery Proxy...");
            try {
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(probeEndpointAddress));
                discoveryEndpoint.IsSystemEndpoint = false;

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(), new EndpointAddress(announcementEndpointAddress));

                proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);
                proxyServiceHost.AddServiceEndpoint(announcementEndpoint);

                proxyServiceHost.Open();

                logger.Info("Proxy Service started.");
            } catch (CommunicationException e) {
                logger.Exception(e);
            } catch (TimeoutException e) {
                logger.Exception(e);
            }
        }

        public void Stop() {
            if (proxyServiceHost.State != CommunicationState.Closed) {
                logger.Info("Aborting the Proxy service...");
                proxyServiceHost.Abort();
            }
        }
    }
}
