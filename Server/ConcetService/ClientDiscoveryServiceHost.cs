using hsfl.ceho5518.vs.server;
using hsfl.ceho5518.vs.server.Sate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.LoggerService;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    public class ClientDiscoveryServiceHost {
        private Uri baseAddress;
        private Uri announcementEndpointAddress;
        private ServiceHost serviceHost;

        public ClientDiscoveryServiceHost() {
            this.baseAddress = new Uri("net.tcp://localhost:9002/ClientDiscoveryService/" + GlobalState.GetInstance().ServerId);
            this.announcementEndpointAddress = new Uri("net.tcp://localhost:9021/Announcement");
            this.serviceHost = new ServiceHost(typeof(ServerDiscoveryService), baseAddress);
        }

        public void Start() {
            Logger.Info("Starting Client Discovery Host...");
            try {
                // Set Behavior
                ServiceMetadataBehavior smb = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null) {
                    smb = new ServiceMetadataBehavior();
                }
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                serviceHost.Description.Behaviors.Add(smb);
                serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                ServiceEndpoint netTcpEndpoint = serviceHost.AddServiceEndpoint(typeof(IServerDiscoveryService),
                    new NetTcpBinding(), string.Empty);

                // Create an announcement endpoint, which points to the Announcement Endpoint hosted by the proxy service.
                AnnouncementEndpoint announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(),
                    new EndpointAddress(announcementEndpointAddress));

                ServiceDiscoveryBehavior serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
                serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);

                // Make the service discoverable
                serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);

                serviceHost.Open();

                Logger.Info($"Client Service started at {baseAddress}");

            } catch (CommunicationException e) {
                Logger.Exception(e);
                Logger.Error($"Failed to load ClientDiscoveryHost. {e.Message}");
            } catch (TimeoutException e) {
                Logger.Exception(e);
                Logger.Error($"Failed to load ClientDiscoveryHost. {e.Message}");
            }
        }

        public void Stop() {
            if (serviceHost.State != CommunicationState.Closed) {
                Logger.Info("Aborting the Client service...");
                serviceHost.Abort();
            }
        }
    }
}