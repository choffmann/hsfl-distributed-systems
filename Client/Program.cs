using hsfl.ceho5518.vs.Client.Services;
using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.Client {
    internal class Program {
        private static ILogger logger = Logger.Instance;
        static void Main(string[] args) {
            // Set Text to UTF-8 to use Emojis and Spinners
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Set LogLevel
            logger.LogLevel = LogLevel.Info;

            var discovery = new DiscoveryMaster(new Uri("net.tcp://localhost:8001/Probe"));
            discovery.SetupProxy();

            Console.ReadLine();
        }
    }
}
