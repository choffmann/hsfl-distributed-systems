using hsfl.ceho5518.vs.LoggerService;
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
        private readonly ILogger logger = LoggerService.Logger.Instance;
        private readonly List<PluginContract.Plugin> pluginsList = new List<PluginContract.Plugin>();
        private readonly string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        public PluginService() {
            AnsiConsole.Status().Spinner(Spinner.Known.Moon).Start(
            $"Register Plugins...", ctx => {
                this.logger.Debug($"Plugin path is {this.pluginPath}");
                // Load Plugins
                LoadPlugins();
                ctx.Status("Startup Plugins...");

                // Run the Startup Lifecycle
                OnStartup();
            });
        }

        public PluginService(string pluginPath) {
            this.pluginPath = pluginPath;
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
            this.logger.Debug($"Load plugins from path: [bold gray]{this.pluginPath}[/]");
            try {
                string[] dlls = Directory.GetFiles(this.pluginPath, "*.dll");
                foreach (string dll in dlls) {
                    var ass = Assembly.LoadFrom(dll);
                    var plugins = ass.GetTypes().Where(w => typeof(PluginContract.Plugin).IsAssignableFrom(w));
                    foreach (var plugin in plugins) {
                        var activatorPlugin = (Activator.CreateInstance(plugin) as PluginContract.Plugin);
                        OnInit(activatorPlugin);
                    }
                }
            } catch (DirectoryNotFoundException ex) {
                this.logger.Exception(ex);
                this.logger.Error($"Failed to load plugins. [bold red]{ex.Message}[/]");
            } catch (ReflectionTypeLoadException ex) {
                this.logger.Exception(ex);
                this.logger.Error($"Failed to load plugins. [bold red]{ex.Message}[/]");
            }
            if (LoadedPlugins() > 0) {
                this.logger.Success($"Successfully load [bold green]{LoadedPlugins()}[/] plugins");
            } else {
                this.logger.Info("No plugins loaded");
            }
        }

        // Total amount of loaded plugins
        public int LoadedPlugins() {
            return this.pluginsList.Count;
        }

        public void ReloadPlugins() {
            this.logger.Info("Reloading plugins...");
            this.pluginsList.Clear();
            LoadPlugins();
            this.logger.SuccessEmoji("Reloading plugins successfully");
        }

        private void OnInit(Plugin plugin) {
            try {
                this.logger.Info($"Register plugin [springgreen3]{plugin.Name}[/]");
                plugin.OnInit();
                this.logger.Success($"Successfully register plugin {plugin.Name}");
                this.pluginsList.Add(plugin);
            } catch(Exception ex) {
                this.logger.Exception(ex);
                this.logger.Warning($"Failed to register plugin [bold springgreen3]{plugin.Name}[/]. [bold red]{ex.Message}[/]. System will ignore plugin [bold springgreen3]{plugin.Name}[/]");
            }
            
        }

        private void OnStartup() {
            foreach (var plugin in this.pluginsList) {
                try {
                    this.logger.Info($"Start Plugin {plugin.Name}");
                    plugin.OnStartup();
                    this.logger.Success($"Successfully start plugin [bold springgreen3]{plugin.Name}[/]");
                } catch (Exception ex) {
                    this.logger.Exception(ex);
                    this.logger.Error($"Failed to startup plugin {plugin.Name}. {ex.Message}");
                }
            }
        }

        public void OnStop() {
            foreach (var plugin in this.pluginsList) {
                this.logger.Info($"Deregister plugin [gray]{plugin.Name}[/]");
                plugin.OnStop();
            }
            this.logger.Info("Startup Plugins...");
            this.logger.Success($"Successfully start [bold green]{LoadedPlugins()}[/] plugins");
        }
    }
}
