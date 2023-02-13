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
        private ILogger logger = LoggerService.Logger.Instance;
        private List<PluginContract.Plugin> pluginsList = new List<PluginContract.Plugin>();
        private string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        public PluginService() {
            AnsiConsole.Status().Spinner(Spinner.Known.Moon).Start(
            $"Register Plugins...", ctx => {
                logger.Debug($"Plugin path is {pluginPath}");
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
            logger.Debug($"Load plugins from path: [bold gray]{pluginPath}[/]");
            try {
                var dlls = Directory.GetFiles(pluginPath, "*.dll");
                foreach (var dll in dlls) {
                    var ass = Assembly.LoadFrom(dll);
                    var plugins = ass.GetTypes().Where(w => typeof(PluginContract.Plugin).IsAssignableFrom(w));
                    foreach (var plugin in plugins) {
                        var activatorPlugin = (Activator.CreateInstance(plugin) as PluginContract.Plugin);
                        OnInit(activatorPlugin);
                    }
                }
            } catch (DirectoryNotFoundException ex) {
                logger.Exception(ex);
                logger.Error($"Failed to load plugins. [bold red]{ex.Message}[/]");
            } catch (ReflectionTypeLoadException ex) {
                logger.Exception(ex);
                logger.Error($"Failed to load plugins. [bold red]{ex.Message}[/]");
            }
            if (LoadedPlungins() > 0) {
                logger.Success($"Successfully load [bold green]{LoadedPlungins()}[/] plugins");
            } else {
                logger.Info("No plugins loaded");
            }
        }

        // Total amount of loaded plugins
        public int LoadedPlungins() {
            return pluginsList.Count;
        }

        public void ReloadPlugins() {
            logger.Info("Reloading plugins...");
            pluginsList.Clear();
            LoadPlugins();
            logger.SuccessEmoji("Reloading plugins successfully");
        }

        public void OnInit(Plugin plugin) {
            try {
                logger.Info($"Register plugin [springgreen3]{plugin.Name}[/]");
                plugin.OnInit();
                logger.Success($"Successfully register plugin {plugin.Name}");
                pluginsList.Add(plugin);
            } catch(Exception ex) {
                logger.Exception(ex);
                logger.Warning($"Failed to register plugin [bold springgreen3]{plugin.Name}[/]. [bold red]{ex.Message}[/]. System will ignore plugin [bold springgreen3]{plugin.Name}[/]");
            }
            
        }

        public void OnStartup() {
            foreach (var plugin in pluginsList) {
                try {
                    logger.Info($"Start Plugin {plugin.Name}");
                    plugin.OnStartup();
                    logger.Success($"Successfully start plugin [bold springgreen3]{plugin.Name}[/]");
                } catch (Exception ex) {
                    logger.Exception(ex);
                    logger.Error($"Failed to startup plugin {plugin.Name}. {ex.Message}");
                }
            }
        }

        public void OnStop() {
            foreach (var plugin in pluginsList) {
                logger.Info($"Deregister plugin [gray]{plugin.Name}[/]");
                plugin.OnStop();
            }
            logger.Info("Startup Plugins...");
            logger.Success($"Successfully start [bold green]{LoadedPlungins()}[/] plugins");
        }
    }
}
