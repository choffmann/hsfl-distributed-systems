using System;

namespace PluginContract.ClientPlugin {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClientPlugin : Attribute {
        public string CommandName { get; }
        public CommandArgument CommandArgument { get; set; }
        public object CommandArgumentType { get; set; }

        public ClientPlugin(string commandName) {
            this.CommandName = commandName;
            this.CommandArgument = null;
            this.CommandArgumentType = null;
        }
    }
}
