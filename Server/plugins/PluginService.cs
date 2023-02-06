using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.Plugins {
    public class PluginService {
        private List<PluginContract.IPlugin> pluginsList = new List<PluginContract.IPlugin>();
        private string path;

        public PluginService(string path) {
            this.path = path;
            LoadPlugins();
        }

        private void LoadPlugins() {

            var dlls = Directory.GetFiles(path, "*.dll");
            foreach (var dll in dlls) {
                var ass = Assembly.LoadFrom(dll);
                var plugins = ass.GetTypes().Where(w => typeof(PluginContract.IPlugin).IsAssignableFrom(w));
                foreach (var plugin in plugins) {
                    AnsiConsole.MarkupLine($"Register plugin [gray]{plugin.Name}[/]");
                    pluginsList.Add(Activator.CreateInstance(plugin) as PluginContract.IPlugin);
                }
            }

            // Run the Startup Lifecycle
            OnStart();
        }

        // Total amount of loaded plugins
        public int LoadedPlungins() {
            return pluginsList.Count;
        }

        public void ReloadPlugins() {
            AnsiConsole.WriteLine("Reloading plugins...");
            pluginsList.Clear();
            LoadPlugins();
            AnsiConsole.WriteLine("Reloading plugins successfully");
        }

        public void OnStart() {
            foreach (var plugin in pluginsList) {
                plugin.OnStart();
            }
        }

        public void OnStop() {
            foreach (var plugin in pluginsList) {
                AnsiConsole.MarkupLine($"Deregister plugin [gray]{plugin.Name}[/]");
                plugin.OnStop();
            }
        }
    }
}
