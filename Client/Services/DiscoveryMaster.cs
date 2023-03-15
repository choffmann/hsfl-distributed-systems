using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.LoggerService;
using System.Reflection;
using hsfl.ceho5518.vs.server.ConcreatService;

namespace hsfl.ceho5518.vs.Client.Services {
    public interface IDiscoveryMaster {
        Uri ProbeEndpointAddress { get; set; }
        EndpointAddress SetupProxy();
    }

    public class DiscoveryMaster : IDiscoveryMaster {
        private readonly ILogger logger = Logger.Instance;
        private DiscoveryEndpoint discoveryEndpoint;
        private DiscoveryClient discoveryClient;
        public Uri ProbeEndpointAddress { get; set; }


        /*public DiscoveryMaster() {
            var probeEndpointAddress = this.ProbeEndpointAddress;
            if (probeEndpointAddress != null)
                this.discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(probeEndpointAddress));
            this.discoveryClient = new DiscoveryClient(this.discoveryEndpoint);
        }*/
        public EndpointAddress SetupProxy() {
            try {
                this.discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(this.ProbeEndpointAddress));
                this.discoveryClient = new DiscoveryClient(this.discoveryEndpoint);
                var findResponse = this.discoveryClient.Find(new FindCriteria(typeof(IClientDiscoveryService)));
                return findResponse.Endpoints[0].Address;
            }
            catch (TargetInvocationException) {
                this.logger.Error("[bold]No Master found in Network.[/]");
                throw new Exception("Unable to connect to master system");
            }
        }
    }
}
