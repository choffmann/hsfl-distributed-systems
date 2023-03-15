using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using hsfl.ceho5518.vs.server.Sate;
using hsfl.ceho5518.vs.server.ConcreatService;
using hsfl.ceho5518.vs.server.State;
using hsfl.ceho5518.vs.LoggerService;
using Spectre.Console;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    public class DiscoveryProxy {
        private readonly ILogger logger = Logger.Instance;
        private readonly DiscoveryClient discoveryClient;

        public DiscoveryProxy(Uri probeEndpointAddress) {
            var discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(probeEndpointAddress));
            this.discoveryClient = new DiscoveryClient(discoveryEndpoint);
        }

        public void SetupProxy() {
            AnsiConsole.Status().Spinner(Spinner.Known.Earth).Start("[yellow]Try to find Master in Network[/]", ctx => {
                try {
                    var findResponse = this.discoveryClient.Find(new FindCriteria(typeof(IServerDiscoveryService)));
                    this.logger.Info("Found the [bold]Master[/] in network, system become a Worker");
                    ctx.Status("[yellow]Start Server as Worker[/]");
                    SetupWorkerDiscovery(findResponse.Endpoints[0].Address);
                }
                catch (TargetInvocationException) {
                    this.logger.Info("[bold]No Master found in Network.[/] Setting this instance to [bold grey]Master[/]");
                    ctx.Status("[yellow]Start Server as Master[/]");
                    SetupMasterDiscovery();
                }
                catch (Exception ex) {
                    this.logger.Error("Unexpected Error on creating DiscoveryProxy. [bold red]Shutdown the Application...[/]");
                    this.logger.Exception(ex);
                    Console.ReadLine();
                    Environment.Exit(100);
                }
            });
        }

        private void SetupMasterDiscovery() {
            GlobalState.GetInstance().ServerState = ServerState.MASTER;

            var proxyHost = new DiscoveryProxyHost();
            var discoveryHost = new ServerDiscoveryServiceHost();
            var clientHost = new ClientDiscoveryServiceHost();
            proxyHost.Start();
            discoveryHost.Start();
            clientHost.Start();

            if (discoveryHost.Status().Equals(CommunicationState.Opened) && clientHost.Status().Equals(CommunicationState.Opened)) {
                this.logger.Debug($"DiscoveryHost CommunicationState is {discoveryHost.Status()}");
                this.logger.Debug($"ClientHost CommunicationState is {clientHost.Status()}");
                this.logger.Success($"Initialized Master successfully");
            } else {
                this.logger.Error("System can't start the Server. [bold red]Shutdown the Application...[/]");
                this.logger.Debug($"DiscoveryHost CommunicationState is {discoveryHost.Status()}");
                this.logger.Debug($"ClientHost CommunicationState is {clientHost.Status()}");
                Console.ReadLine();
                Environment.Exit(100);
            }
        }

        private void SetupWorkerDiscovery(EndpointAddress endpointAddress) {
            GlobalState.GetInstance().ServerState = ServerState.WORKER;
            var serverDiscovery = new InvokeServerDiscovery();
            serverDiscovery.Connect(endpointAddress);
            
            this.logger.Success($"Worker initialization successful");
        }
    }
}
