using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    public class DiscoveryProxyHost {
        private Uri probeEndpointAddress;
        private Uri announcementEndpointAddress;
        private ServiceHost proxyServiceHost;

        public DiscoveryProxyHost() {
            this.probeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
            this.announcementEndpointAddress = new Uri("net.tcp://localhost:9021/Announcement");
            this.proxyServiceHost = new ServiceHost(new DiscoveryProxyService());
        }

        public void Start() {
            try {
                // Add DiscoveryEndpoint to receive Probe and Resolve messages
                DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(probeEndpointAddress));
                discoveryEndpoint.IsSystemEndpoint = false;

                // Add AnnouncementEndpoint to receive Hello and Bye announcement messages
                AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(), new EndpointAddress(announcementEndpointAddress));

                proxyServiceHost.AddServiceEndpoint(discoveryEndpoint);
                proxyServiceHost.AddServiceEndpoint(announcementEndpoint);

                proxyServiceHost.Open();

                Console.WriteLine("Proxy Service started.");
            } catch (CommunicationException e) {
                Console.WriteLine(e.Message);
            } catch (TimeoutException e) {
                Console.WriteLine(e.Message);
            }
        }

        public void Stop() {
            if (proxyServiceHost.State != CommunicationState.Closed) {
                Console.WriteLine("Aborting the Proxy service...");
                proxyServiceHost.Abort();
            }
        }
    }
}
