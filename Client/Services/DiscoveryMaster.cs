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
        void SetupProxy();
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

        public void SetupProxy() {
            AnsiConsole.Status().Spinner(Spinner.Known.BouncingBar).Start("[yellow]Try to find Master in Network[/]", ctx => {
                try {
                    this.discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(this.ProbeEndpointAddress));
                    this.discoveryClient = new DiscoveryClient(this.discoveryEndpoint);
                    var findResponse = this.discoveryClient.Find(new FindCriteria(typeof(IClientDiscoveryService)));
                    this.logger.Info("Found the [bold]Master[/] in network");
                    ctx.Status("[yellow]Connecting to Master...[/]");
                    SetupClient(findResponse.Endpoints[0].Address);
                }
                catch (TargetInvocationException) {
                    this.logger.Error("[bold]No Master found in Network.[/]");
                }
            });
        }
        private void SetupClient(EndpointAddress endpointAddress) {
            var invokeService = new InvokeClientDiscovery();
            invokeService.Connect(endpointAddress);

            this.logger.Success("Client initialization successful");
        }
    }
}
