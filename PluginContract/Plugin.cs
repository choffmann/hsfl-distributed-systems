﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginContract.ClientPlugin;

namespace PluginContract {
    public abstract class Plugin : ILifecycle {
        public string Name { get; }
        public virtual string CommandName { get; }
        public PluginContract.Logger Logger { get; } = PluginContract.Logger.Instance;

        public Plugin() {
        }

        public Plugin(string pluginName) {
            this.Name = pluginName;
            this.Logger.PluginName = pluginName;
        }
        public abstract void OnServerExecute(string[] input);

        public virtual void OnServerInit() { }

        public virtual void OnServerStartup() { }

        public virtual void OnServerStop() { }
    }
}
