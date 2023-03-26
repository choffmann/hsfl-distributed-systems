using System;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Reflection;
using hsfl.ceho5518.vs.server.ConcreatService;
using hsfl.ceho5518.vs.server.ServiceContracts.ClientDiscoveryService;

namespace hsfl.ceho5518.vs.Client.Services {
    public interface IDiscoveryMaster {
        Uri ProbeEndpointAddress { get; set; }
        EndpointAddress SetupProxy();
    }

    public class DiscoveryMaster : IDiscoveryMaster {
        private DiscoveryEndpoint discoveryEndpoint;
        private DiscoveryClient discoveryClient;
        public Uri ProbeEndpointAddress { get; set; }
        
        public EndpointAddress SetupProxy() {
            try {
                this.discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(this.ProbeEndpointAddress));
                this.discoveryClient = new DiscoveryClient(this.discoveryEndpoint);
                var findResponse = this.discoveryClient.Find(new FindCriteria(typeof(IClientDiscoveryService)));
                return findResponse.Endpoints[0].Address;
            }
            catch (TargetInvocationException) {
                throw new Exception("Es wurde kein Master im Netzwerk gefunden.");
            }
        }
    }
}
