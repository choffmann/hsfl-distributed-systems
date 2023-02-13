using hsfl.ceho5518.vs.LoggerService;
using hsfl.ceho5518.vs.server.Sate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.ConcreatService {
    public class InvokeServerDiscovery {
        public static void InvokeDiscoveryService(EndpointAddress endpointAddress) {
            // Create a client
            ServerDiscoveryServiceClient client = new ServerDiscoveryServiceClient(new NetTcpBinding(), endpointAddress);
            Logger.Info($"Invoking CalculatorService at {endpointAddress.Uri}");

            client.Connect(GlobalState.GetInstance().ServerId.ToString());
            Logger.Info($"Hello to Master from Worker {GlobalState.GetInstance().ServerId}");

            client.Close();
        }
    }
}