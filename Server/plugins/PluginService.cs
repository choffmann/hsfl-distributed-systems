using hsfl.ceho5518.vs.server.LoggerService;
using PluginContract;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.server.Plugins {
    public class PluginService {
        private List<PluginContract.IPlugin> pluginsList = new List<PluginContract.IPlugin>();
        private string path;

        public PluginService(string path) {
            this.path = path;
            AnsiConsole.Status().Spinner(Spinner.Known.Moon).Start(
            $"Register Plugins...", ctx => {
                // Load Plugins
                LoadPlugins();
                ctx.Status("Startup Plugins...");

                // Run the Startup Lifecycle
                OnStartup();
            });
        }

        private void LoadPlugins() {
            Logger.Debug($"Load plugins from path: [bold gray]{path}[/]");
            try {
                var dlls = Directory.GetFiles(path, "*.dll");
                foreach (var dll in dlls) {
                    var ass = Assembly.LoadFrom(dll);
                    var plugins = ass.GetTypes().Where(w => typeof(PluginContract.IPlugin).IsAssignableFrom(w));
                    foreach (var plugin in plugins) {
                        Logger.Info($"Register plugin [springgreen3]{plugin.Name}[/]");
                        var activatorPlugin = (Activator.CreateInstance(plugin) as PluginContract.IPlugin);
                        pluginsList.Add(activatorPlugin);
                    }
                }
            } catch (DirectoryNotFoundException ex) {
                Logger.Exception(ex);
                Logger.Error($"Failed to load plugins. [bold red]{ex.Message}[/]");
            } catch (ReflectionTypeLoadException ex) {
                Logger.Exception(ex);
                Logger.Error($"Failed to load plugins. [bold red]{ex.Message}[/]");
            }

            Logger.Info("Register Plugins...");
            if (LoadedPlungins() > 0) {
                Logger.Success($"Successfully load [bold green]{LoadedPlungins()}[/] plugins");
            } else {
                Logger.Info("No plugins loaded");
            }
        }

        // Total amount of loaded plugins
        public int LoadedPlungins() {
            return pluginsList.Count;
        }

        public void ReloadPlugins() {
            Logger.Info("Reloading plugins...");
            pluginsList.Clear();
            LoadPlugins();
            Logger.SuccessEmoji("Reloading plugins successfully");
        }

        public void OnStartup() {
            foreach (var plugin in pluginsList) {
                Logger.Info($"Start Plugin {plugin.Name}");
                plugin.OnStartup();
                Thread.Sleep(2000);
            }
        }

        public void OnStop() {
            foreach (var plugin in pluginsList) {
                Logger.Info($"Deregister plugin [gray]{plugin.Name}[/]");
                plugin.OnStop();
            }
            Logger.Info("Startup Plugins...");
            Logger.Success($"Successfully start [bold green]{LoadedPlungins()}[/] plugins");
        }
    }
}
