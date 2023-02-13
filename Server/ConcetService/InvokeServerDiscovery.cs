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
        private static ILogger logger = Logger.Instance;
        public static void InvokeDiscoveryService(EndpointAddress endpointAddress) {

            // Create a client
            ServerDiscoveryServiceClient client = new ServerDiscoveryServiceClient(new NetTcpBinding(), endpointAddress);
            logger.Info($"Invoking CalculatorService at {endpointAddress.Uri}");

            client.Connect(GlobalState.GetInstance().ServerId.ToString());
            logger.Info($"Hello to Master from Worker {GlobalState.GetInstance().ServerId}");

            client.Close();
        }
    }
}
