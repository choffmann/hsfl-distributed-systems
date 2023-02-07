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

namespace Client {
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
            AnsiConsole.Status().Spinner(Spinner.Known.Earth).Start("[yellow]Try to find Master in Network[/]", ctx => {
                try {
                    //FindResponse findResponse = discoveryClient.Find(new FindCriteria(typeof(IClientDiscoveryService)));
                    Logger.Info("Found the [bold]Master[/] in network, system become a Worker");
                    ctx.Status("[yellow]Start Server as Worker[/]");
                    //SetupWorkerDiscovery(findResponse.Endpoints[0].Address);
                } catch (TargetInvocationException) {
                    Logger.Info("[bold]No Master found in Network.[/] Setting this instance to [bold grey]Master[/]");
                }
            });
        }
    }
}
