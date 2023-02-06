using hsfl.ceho5518.vs.server.DiscoveryProxy;
using hsfl.ceho5518.vs.server.Plugins;
using hsfl.ceho5518.vs.server.Sate;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server {
    internal class Program {
        static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            GlobalState state = GlobalState.GetInstance();
            EndpointAddress endpointAddress = null;

            // Create a DiscoveryEndpoint that points to the DiscoveryProxy
            Uri probeEndpointAddress = new Uri("net.tcp://localhost:8001/Probe");
            DiscoveryEndpoint discoveryEndpoint = new DiscoveryEndpoint(new NetTcpBinding(), new EndpointAddress(probeEndpointAddress));

            // Create a DiscoveryClient passing in the discovery endpoint
            DiscoveryClient discoveryClient = new DiscoveryClient(discoveryEndpoint);
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .Start(
                $"[yellow]Starting Server...[/]", ctx => {
                    Thread.Sleep(2000);
                    LogMessage("Starting Server...");
                    ctx.Status("[yellow]Try to find Master in Network[/]");
                    try {
                        // Search for services that implement ICalculatorService
                        FindResponse findResponse = discoveryClient.Find(new FindCriteria(typeof(ICalculatorService)));
                        LogMessage("Found the [bold]Master[/] in network, system become a worker");
                        ctx.Status("[yellow]Start Server as Worker[/]");
                        if (findResponse.Endpoints.Count > 0) {
                            state.serverState = State.ServerState.WORKER;
                            StartWorker(findResponse.Endpoints[0].Address);
                        }
                    } catch (TargetInvocationException) {
                        LogMessage("[bold]No Master found in Network.[/] Setting this instance to [bold grey]Master[/]");
                        ctx.Status("[yellow]Start Server as Master[/]");
                        state.serverState = State.ServerState.MASTER;
                        StartMaster();
                    }
                    LogMessage(":party_popper: [green]Initialization successfully[/]");
                });

            Console.WriteLine("");
            Console.WriteLine("Press <Enter> to exit");
            Console.ReadLine();
        }

        private static void StartMaster() {
            DiscoveryProxyHost proxyHost = new DiscoveryProxyHost();
            CalculatorHost calculatorHost = new CalculatorHost();

            proxyHost.Start();
            calculatorHost.Start();
        }

        private static void StartWorker(EndpointAddress endpointAddress) {
            InvokeDiscoveryProxy.InvokeCalculatorService(endpointAddress);
        }

        private static void LoadPlugins() {
            PluginService pluginService = new PluginService("C:\\Users\\hoffmann\\Documents\\FH Flensburg\\7. Semester\\Verteilte Systeme\\VS-Hausarbeit\\DemoPlugin\\bin\\Debug");
        }

        private static void LogMessage(string message) {
            AnsiConsole.MarkupLine($"[grey][[LOG]][/] {message}");
        }
    }
}
