using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginContract {
    public interface IPlugin {
        string Name { get; }

        // Lifecycle
        void OnStart();
        void OnStop();
    }
}
