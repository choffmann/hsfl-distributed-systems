using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPlugin
{
    public class DemoPlugin : PluginContract.IPlugin {
        public string Name => "DemoPlugin";

        public void OnInit() {
            throw new NotImplementedException("Ein Fehler beim Aufruf von OnInit()");
            Thread.Sleep(2000);
        }

        public void OnStartup() {
            Thread.Sleep(2000);
        }

        public void OnStop() {
        }
    }
}
