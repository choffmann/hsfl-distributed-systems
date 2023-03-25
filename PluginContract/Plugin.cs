using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginContract.ClientPlugin;

namespace PluginContract {
    public abstract class Plugin : ILifecycle {
        public string Name { get; }
        public virtual string CommandName { get; }
        public virtual CommandArgument CommandArgument { get; set; }
        public virtual object CommandArgumentType { get; set; }
        public PluginContract.Logger Logger { get; } = PluginContract.Logger.Instance;

        public Plugin() {
            // Name = pluginName;
            // PluginContract.Logger.Instance.PluginName = pluginName;
        }

        public Plugin(string pluginName) {
            this.Name = pluginName;
            this.Logger.PluginName = pluginName;
        }

        public virtual void OnClientExecute(int value) {}

        public virtual void OnServerInit() { }

        public virtual void OnServerStartup() { }

        public virtual void OnServerStop() { }
        public virtual void OnClientInit() { }
        public virtual void OnClientStart() { }
        public virtual void OnClientStop() { }
    }
}
