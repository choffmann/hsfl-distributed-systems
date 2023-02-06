using hsfl.ceho5518.vs.server.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    // Define a service contract.
    [ServiceContract(Namespace = "http://hsfl.ceho5518.vs.server.DiscoveryProxy")]
    public interface ICalculatorService {
        [OperationContract]
        double Add(double n1, double n2);
        [OperationContract]
        double Subtract(double n1, double n2);
        [OperationContract]
        double Multiply(double n1, double n2);
        [OperationContract]
        double Divide(double n1, double n2);
    }

    // Service class which implements the service contract.
    public class CalculatorService : ICalculatorService {
        public double Add(double n1, double n2) {
            double result = n1 + n2;
            Logger.Info($"Received Add({n1},{n2})");
            Logger.Info($"Return: {result}");
            return result;
        }

        public double Subtract(double n1, double n2) {
            double result = n1 - n2;
            Logger.Info($"Received Subtract({n1},{n2})");
            Logger.Info($"Return: {result}");
            return result;
        }

        public double Multiply(double n1, double n2) {
            double result = n1 * n2;
            Logger.Info($"Received Multiply({n1},{n2})");
            Logger.Info($"Return: {result}");
            return result;
        }

        public double Divide(double n1, double n2) {
            double result = n1 / n2;
            Logger.Info($"Received Divide({n1},{n2})");
            Logger.Info($"Return: {result}");
            return result;
        }
    }
}
