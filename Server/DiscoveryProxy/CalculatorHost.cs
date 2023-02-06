using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    public class CalculatorHost {
        private Uri baseAddress;
        private Uri announcementEndpointAddress;
        private ServiceHost serviceHost;

        public CalculatorHost() {
            this.baseAddress = new Uri("net.tcp://localhost:9002/CalculatorService/" + Guid.NewGuid().ToString());
            this.announcementEndpointAddress = new Uri("net.tcp://localhost:9021/Announcement");
            this.serviceHost = new ServiceHost(typeof(CalculatorService), baseAddress);
        }

        public void Start() {
            try {
                // Set Behavior
                ServiceMetadataBehavior smb = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null) {
                    smb = new ServiceMetadataBehavior();
                }
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                serviceHost.Description.Behaviors.Add(smb);
                serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                ServiceEndpoint netTcpEndpoint = serviceHost.AddServiceEndpoint(typeof(ICalculatorService),
                    new NetTcpBinding(), string.Empty);

                // Create an announcement endpoint, which points to the Announcement Endpoint hosted by the proxy service.
                AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(),
                    new EndpointAddress(announcementEndpointAddress));

                ServiceDiscoveryBehavior serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
                serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);

                // Make the service discoverable
                serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);

                serviceHost.Open();

                Console.WriteLine("Calculator Service started at {0}", baseAddress);
            } catch (CommunicationException e) {
                Console.WriteLine(e.Message);
            } catch (TimeoutException e) {
                Console.WriteLine(e.Message);
            }
        }

        public void Stop() {
            if (serviceHost.State != CommunicationState.Closed) {
                Console.WriteLine("Aborting the Calculator service...");
                serviceHost.Abort();
            }
        }
    }
}
