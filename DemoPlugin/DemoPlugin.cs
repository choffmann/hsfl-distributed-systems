using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPlugin
{
    public class DemoPlugin : PluginContract.IPlugin {
        public string Name => "DemoPlugin";

        public void OnStart() {
            Console.WriteLine("DemoPlugin::OnStart()");
            FooBar();
            Console.WriteLine(ReturnString("value +"));
        }

        public void OnStop() {
            Console.WriteLine("DemoPlugin::OnStop()");
        }

        public void FooBar() {
            Console.WriteLine("  Die Methode FooBar, welche nicht vom Server bekannt ist.");
        }

        public string ReturnString(string s) {
            return s + " das wurde angegängt!";
        }
    }
}
