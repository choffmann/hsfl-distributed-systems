using hsfl.ceho5518.vs.LoggerService;
using PluginContract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPlugin
{
    public class DemoPlugin : PluginContract.Plugin {

        public DemoPlugin(): base("DemoPlugin") { }

        public override void OnInit() {
            Logger.Info($"Startup {Name}");
            throw new NotImplementedException("Ein Fehler beim Aufruf von OnInit() :smiling_face_with_horns:");
            Thread.Sleep(2000);
        }

        public override void OnStartup() {
            Thread.Sleep(2000);
        }
    }
}
