using System.Collections.Generic;
using System.Runtime.Serialization;

namespace hsfl.ceho5518.vs.server.ServiceContracts.Model {
    [DataContract]
    public class PluginStatus {
        [DataMember]
        public List<Plugin> Plugins { get; set; }
        
        public PluginStatus(List<Plugin> plugins) {
            this.Plugins = plugins;
        }
    }

    [DataContract]
    public class Plugin {
        [DataMember]
        public bool Activated { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Size { get; set; }
    }
}
