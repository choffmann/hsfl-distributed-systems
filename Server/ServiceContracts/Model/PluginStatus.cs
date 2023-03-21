using System.Collections.Generic;

namespace hsfl.ceho5518.vs.ServiceContracts.Model {
    public class PluginStatus {
        public List<Plugin> Plugins { get; set; }

        public PluginStatus(List<Plugin> plugins) {
            this.Plugins = plugins;
        }
    }

    public class Plugin {
        public bool Activated { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
    }
}
