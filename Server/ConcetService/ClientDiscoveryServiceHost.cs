using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Sate;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    public class ClientDiscoveryServiceHost {
        private readonly ILogger logger = Logger.Instance;
        private readonly Uri baseAddress;
        private readonly Uri announcementEndpointAddress;
        private readonly ServiceHost serviceHost;

        public ClientDiscoveryServiceHost() {
            this.baseAddress = new Uri($"net.tcp://{Environment.MachineName}:9002/ClientDiscoveryService/" + GlobalState.GetInstance().ServerId);
            this.announcementEndpointAddress = new Uri($"net.tcp://{Environment.MachineName}:9021/Announcement");
            this.serviceHost = new ServiceHost(typeof(ServerDiscoveryService), baseAddress);
        }

        public void Start() {
            this.logger.Info("Starting Client Discovery Host...");
            try {
                // Set Behavior
                var smb = this.serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                this.serviceHost.Description.Behaviors.Add(smb);
                this.serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
                this.serviceHost.AddServiceEndpoint(typeof(IServerDiscoveryService), new NetTcpBinding(), string.Empty);

                // Create an announcement endpoint, which points to the Announcement Endpoint hosted by the proxy service.
                var announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(), new EndpointAddress(this.announcementEndpointAddress));
                var serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
                serviceDiscoveryBehavior.AnnouncementEndpoints.Add(announcementEndpoint);

                // Make the service discoverable
                this.serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior);
                this.serviceHost.Open();

                this.logger.Info($"Client Service started at {this.baseAddress}");
            }
            catch (CommunicationException e) {
                this.logger.Exception(e);
                this.logger.Error($"Failed to load ClientDiscoveryHost. {e.Message}");
            }
            catch (TimeoutException e) {
                this.logger.Exception(e);
                this.logger.Error($"Failed to load ClientDiscoveryHost. {e.Message}");
            }
        }

        public CommunicationState Status() {
            return this.serviceHost.State;
        }

        public void Stop() {
            if (this.serviceHost.State == CommunicationState.Closed) 
                return;
            this.logger.Info("Aborting the Client service...");
            this.serviceHost.Abort();
        }
    }
}
