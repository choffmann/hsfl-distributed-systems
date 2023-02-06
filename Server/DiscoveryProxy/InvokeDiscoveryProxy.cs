using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    public class InvokeDiscoveryProxy {
        public static void InvokeCalculatorService(EndpointAddress endpointAddress) {
            // Create a client
            CalculatorServiceClient client = new CalculatorServiceClient(new NetTcpBinding(), endpointAddress);
            Console.WriteLine("Invoking CalculatorService at {0}", endpointAddress.Uri);
            Console.WriteLine();

            double value1 = 100.00D;
            double value2 = 15.99D;

            // Call the Add service operation.
            double result = client.Add(value1, value2);
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result);

            // Call the Subtract service operation.
            result = client.Subtract(value1, value2);
            Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result);

            // Call the Multiply service operation.
            result = client.Multiply(value1, value2);
            Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result);

            // Call the Divide service operation.
            result = client.Divide(value1, value2);
            Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result);
            Console.WriteLine();

            // Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }
    }
}
