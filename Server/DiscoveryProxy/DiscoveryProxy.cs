using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using hsfl.ceho5518.vs.server.Sate;
using hsfl.ceho5518.vs.server.LoggerService;
using hsfl.ceho5518.vs.server.ConcreatService;
using hsfl.ceho5518.vs.server.State;
using Spectre.Console;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    public class DiscoveryProxy {
        Uri probeEndpointAddress;
        DiscoveryEndpoint discoveryEndpoint;
        DiscoveryClient discoveryClient;

        public DiscoveryProxy(Uri probeEndpointAddress) {
            this.probeEndpointAddress = probeEndpointAddress;
            this.discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(this.probeEndpointAddress));
            this.discoveryClient = new DiscoveryClient(this.discoveryEndpoint);
        }

        public void SetupProxy() {
            AnsiConsole.Status().Spinner(Spinner.Known.Earth).Start("[yellow]Try to find Master in Network[/]", ctx => {
                try {
                    FindResponse findResponse = discoveryClient.Find(new FindCriteria(typeof(IServerDiscoveryService)));
                    Logger.Info("Found the [bold]Master[/] in network, system become a Worker");
                    ctx.Status("[yellow]Start Server as Worker[/]");
                    SetupWorkerDiscovery(findResponse.Endpoints[0].Address);
                } catch (TargetInvocationException) {
                    Logger.Info("[bold]No Master found in Network.[/] Setting this instance to [bold grey]Master[/]");
                    ctx.Status("[yellow]Start Server as Master[/]");
                    SetupMasterDiscovery();
                }
            });
        }

        private void SetupMasterDiscovery() {
            GlobalState.GetInstance().ServerState = ServerState.WORKER;

            DiscoveryProxyHost proxyHost = new DiscoveryProxyHost();
            ServerDiscoveryServiceHost discoveryHost = new ServerDiscoveryServiceHost();

            proxyHost.Start();
            discoveryHost.Start();

            Logger.Success($"Initializat Master successfully");
        }

        private void SetupWorkerDiscovery(EndpointAddress endpointAddress) {
            GlobalState.GetInstance().ServerState = ServerState.MASTER;
            InvokeServerDiscovery.InvokeDiscoveryService(endpointAddress);

            Logger.Success($"Initializat Worker successfully");
        }
    }
}
