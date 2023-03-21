using hsfl.ceho5518.vs.LoggerService;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using hsfl.ceho5518.vs.server.ServiceContracts.Model;
using hsfl.ceho5518.vs.server.ServiceContracts.Observer;
using Plugin = PluginContract.Plugin;

namespace hsfl.ceho5518.vs.server.Plugins {
    public class PluginService : IPluginObserver {
        private readonly ILogger logger = LoggerService.Logger.Instance;
        static PluginService instance;
        private readonly List<PluginContract.Plugin> pluginsList = new List<PluginContract.Plugin>();
        private readonly string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        private PluginObserver _pluginObserver = PluginObserver.GetInstance();

        private PluginService() {
            this._pluginObserver.AddObserver(this);
        }

        public static PluginService GetInstance() {
            return instance ??= new PluginService();
        }

        public void Startup() {
            AnsiConsole.Status().Spinner(Spinner.Known.Moon).Start(
                $"Register Plugins...", ctx => {
                    this.logger.Debug($"Plugin path is {this.pluginPath}");
                    CreatePluginFolder();

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
                if (dlls.Length == 0) {
                    this.logger.Info("No Plugins available");
                    return;
                }

                foreach (string dll in dlls) {
                    var ass = Assembly.LoadFrom(dll);
                    AddPlugin(ass);
                }
            }
            catch (DirectoryNotFoundException e) {
                this.logger.Error($"It seems like the path is incomplete {this.pluginPath}");
            }
            catch (ReflectionTypeLoadException ex) {
                this.logger.Error($"Failed to load plugin.");
            }

            if (LoadedPlugins() > 0) {
                this.logger.Success($"Load [bold green]{LoadedPlugins()}[/] plugins");
            } else {
                this.logger.Info("No plugins loaded");
            }
        }

        // Total amount of loaded plugins
        public int LoadedPlugins() {
            return this.pluginsList.Count;
        }

        public void AddPlugin(Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            var plugins = assembly.GetTypes().Where(w => typeof(PluginContract.Plugin).IsAssignableFrom(w));
            foreach (var plugin in plugins) {
                var activatorPlugin = (Activator.CreateInstance(plugin) as PluginContract.Plugin);
                OnInit(activatorPlugin);
            }
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
            }
            catch (Exception ex) {
                this.logger.Exception(ex);
                this.logger.Warning(
                    $"Failed to register plugin [bold springgreen3]{plugin.Name}[/]. [bold red]{ex.Message}[/]. System will ignore plugin [bold springgreen3]{plugin.Name}[/]");
            }
        }

        private void OnStartup() {
            foreach (var plugin in this.pluginsList) {
                try {
                    this.logger.Info($"Start Plugin {plugin.Name}");
                    plugin.OnStartup();
                    this.logger.Success($"Plugin [bold springgreen3]{plugin.Name}[/] started successfully");
                }
                catch (Exception ex) {
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

        private void CreatePluginFolder() {
            if (!Directory.Exists(this.pluginPath)) {
                Directory.CreateDirectory(this.pluginPath);
            }
        }

        private string GetNameFromDll(byte[] assemblyBytes) {
            var assembly = Assembly.Load(assemblyBytes);
            var plugins = assembly.GetTypes().Where(w => typeof(PluginContract.Plugin).IsAssignableFrom(w));
            foreach (var plugin in plugins) {
                var activatorPlugin = (Activator.CreateInstance(plugin) as PluginContract.Plugin);
                return activatorPlugin.Name;
            }
            throw new Exception("Fehler beim laden von neuem Plugin");
        }

        public void OnPluginUpload(PluginObserver plugin) {
            string name = GetNameFromDll(plugin.Assembly);
            this.logger.Debug($"Plugin [grey]{name}[/] wurde vom Client hochgeladen und wird registriert");
            string path = this.pluginPath + "\\" + name + ".dll";
            File.WriteAllBytes(path, plugin.Assembly);
            var assembly = Assembly.LoadFrom(path);
            AddPlugin(assembly);
        }

        public PluginStatus ReportPlugins() {
            var reportList = this.pluginsList.Select(plugin => new ServiceContracts.Model.Plugin {
                    Name = plugin.Name, 
                    Size = new FileInfo(this.pluginPath + "\\" + plugin.Name + ".dll").Length, 
                    Activated = true 
                }).ToList();
            return new PluginStatus(reportList);
        }
    }
}
