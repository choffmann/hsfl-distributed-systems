using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginContract {
    public abstract class Plugin: ILifecycle {
        public string Name { get; }
        public PluginContract.Logger Logger { get; } = PluginContract.Logger.Instance;

        public Plugin() {
            // Name = pluginName;
            // PluginContract.Logger.Instance.PluginName = pluginName;
        }

        public Plugin(string pluginName) {
            Name = pluginName;
            Logger.PluginName = pluginName;
        }

        public virtual void OnInit() {
        }

        public virtual void OnStartup() {
        }

        public virtual void OnStop() {
        }
    }
}
