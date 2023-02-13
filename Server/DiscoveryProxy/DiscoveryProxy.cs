﻿using System;
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
        private ILogger logger = Logger.Instance;
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
                    logger.Info("Found the [bold]Master[/] in network, system become a Worker");
                    ctx.Status("[yellow]Start Server as Worker[/]");
                    SetupWorkerDiscovery(findResponse.Endpoints[0].Address);
                } catch (TargetInvocationException) {
                    logger.Info("[bold]No Master found in Network.[/] Setting this instance to [bold grey]Master[/]");
                    ctx.Status("[yellow]Start Server as Master[/]");
                    SetupMasterDiscovery();
                }
            });
        }

        private void SetupMasterDiscovery() {
            GlobalState.GetInstance().ServerState = ServerState.WORKER;

            DiscoveryProxyHost proxyHost = new DiscoveryProxyHost();
            ServerDiscoveryServiceHost discoveryHost = new ServerDiscoveryServiceHost();
            ClientDiscoveryServiceHost clientHost = new ClientDiscoveryServiceHost();
            proxyHost.Start();
            discoveryHost.Start();
            clientHost.Start();

            //AnsiConsole.MarkupLine(.ToString());

            if (discoveryHost.Status().Equals(CommunicationState.Faulted) && clientHost.Status().Equals(CommunicationState.Faulted)) {
                logger.Error("System can't start the Server. [bold red]Shutdown the Application...[/]");
                Console.ReadLine();
                Environment.Exit(100);
            }

            // TODO: Check if start is successfully

            logger.Success($"Initialized Master successfully");
        }

        private void SetupWorkerDiscovery(EndpointAddress endpointAddress) {
            GlobalState.GetInstance().ServerState = ServerState.MASTER;
            InvokeServerDiscovery.InvokeDiscoveryService(endpointAddress);

            logger.Success($"Initialized Worker successfully");
        }
    }
}
