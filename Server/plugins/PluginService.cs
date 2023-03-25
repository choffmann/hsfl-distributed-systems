using hsfl.ceho5518.vs.LoggerService;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using hsfl.ceho5518.vs.server.Sate;
using hsfl.ceho5518.vs.server.ServiceContracts.Model;
using Plugin = PluginContract.Plugin;

namespace hsfl.ceho5518.vs.server.Plugins {
    public class PluginService  {
        private readonly ILogger logger = LoggerService.Logger.Instance;
        static PluginService instance;
        public List<Plugin> PluginsList = new List<Plugin>();
        public string PluginPath { get; }= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", GlobalState.GetInstance().ServerId);

        private PluginService() {
        }

        public static PluginService GetInstance() {
            return instance ??= new PluginService();
        }

        public void Startup() {
            AnsiConsole.Status().Spinner(Spinner.Known.Moon).Start(
                $"Register Plugins...", ctx => {
                    this.logger.Debug($"Plugin path is {this.PluginPath}");
                    CreatePluginFolder();

                    // Load Plugins
                    LoadPlugins();
                    ctx.Status("Startup Plugins...");

                    // Run the Startup Lifecycle
                    OnStartup();
                });
        }

        private void LoadPlugins() {
            this.logger.Debug($"Load plugins from path: [bold gray]{this.PluginPath}[/]");
            try {
                string[] dlls = Directory.GetFiles(this.PluginPath, "*.dll");
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
                this.logger.Error($"It seems like the path is incomplete {this.PluginPath}");
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
            return this.PluginsList.Count;
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
            this.PluginsList.Clear();
            LoadPlugins();
            this.logger.SuccessEmoji("Reloading plugins successfully");
        }

        private void OnInit(Plugin plugin) {
            try {
                this.logger.Info($"Register plugin [springgreen3]{plugin.Name}[/]");
                plugin.OnServerInit();
                this.logger.Success($"Successfully register plugin {plugin.Name}");
                this.PluginsList.Add(plugin);
            }
            catch (Exception ex) {
                this.logger.Exception(ex);
                this.logger.Warning(
                    $"Failed to register plugin [bold springgreen3]{plugin.Name}[/]. [bold red]{ex.Message}[/]. System will ignore plugin [bold springgreen3]{plugin.Name}[/]");
            }
        }

        private void OnStartup() {
            foreach (var plugin in this.PluginsList) {
                try {
                    this.logger.Info($"Start Plugin {plugin.Name}");
                    plugin.OnServerStartup();
                    this.logger.Success($"Plugin [bold springgreen3]{plugin.Name}[/] started successfully");
                }
                catch (Exception ex) {
                    this.logger.Exception(ex);
                    this.logger.Error($"Failed to startup plugin {plugin.Name}. {ex.Message}");
                }
            }
        }

        public void OnStop() {
            foreach (var plugin in this.PluginsList) {
                this.logger.Info($"Deregister plugin [gray]{plugin.Name}[/]");
                plugin.OnServerStop();
            }
            this.logger.Info("Startup Plugins...");
            this.logger.Success($"Successfully start [bold green]{LoadedPlugins()}[/] plugins");
        }

        private void CreatePluginFolder() {
            if (!Directory.Exists(this.PluginPath)) {
                Directory.CreateDirectory(this.PluginPath);
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

        public void OnPluginUpload(byte[] assemblyBytes) {
            string name = GetNameFromDll(assemblyBytes);
            this.logger.Debug($"Plugin [grey]{name}[/] wurde vom Client hochgeladen und wird registriert");
            string path = this.PluginPath + "\\" + name + ".dll";
            File.WriteAllBytes(path, assemblyBytes);
            var assembly = Assembly.LoadFrom(path);
            AddPlugin(assembly);
        }

        public PluginStatus ReportPlugins() {
            var reportList = this.PluginsList.Select(plugin => new ServiceContracts.Model.Plugin {
                    Name = plugin.Name, 
                    CommandName = plugin.CommandName,
                    Size = new FileInfo(this.PluginPath + "\\" + plugin.Name + ".dll").Length, 
                    Activated = true 
                }).ToList();
            return new PluginStatus(reportList);
        }
    }
}
