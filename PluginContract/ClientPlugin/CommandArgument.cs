using System;

namespace PluginContract.ClientPlugin {
    public class CommandArgument {
        public int Position { get; }
        public string Template { get; }

        public CommandArgument(int position, string template) {
            this.Position = position;
            this.Template = template;
        }
    }
}
