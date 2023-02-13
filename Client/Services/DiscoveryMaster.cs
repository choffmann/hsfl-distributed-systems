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
    public class DiscoveryMaster {
        Uri probeEndpointAddress;
        DiscoveryEndpoint discoveryEndpoint;
        DiscoveryClient discoveryClient;

        public DiscoveryMaster(Uri probeEndpointAddress) {
            this.probeEndpointAddress = probeEndpointAddress;
            this.discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(this.probeEndpointAddress));
            this.discoveryClient = new DiscoveryClient(this.discoveryEndpoint);
        }

        public void SetupProxy() {
            AnsiConsole.Status().Spinner(Spinner.Known.BouncingBar).Start("[yellow]Try to find Master in Network[/]", ctx => {
                try {
                    FindResponse findResponse = discoveryClient.Find(new FindCriteria(typeof(IClientDiscoveryService)));
                    Logger.Info("Found the [bold]Master[/] in network");
                    ctx.Status("[yellow]Connecting to Master...[/]");
                    //SetupWorkerDiscovery(findResponse.Endpoints[0].Address);
                } catch (TargetInvocationException) {
                    Logger.Error("[bold]No Master found in Network.[/]");
                }
            });
        }
    }
}
