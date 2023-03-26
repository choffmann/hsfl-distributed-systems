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
                $"Registriere Plugins...", ctx => {
                    this.logger.Debug($"Plugin Pfad ist: {this.PluginPath}");
                    CreatePluginFolder();

                    // Load Plugins
                    LoadPlugins();
                    ctx.Status("Plugins werden gestartet...");

                    // Run the Startup Lifecycle
                    OnStartup();
                });
        }

        private void LoadPlugins() {
            this.logger.Debug($"Load plugins from path: [bold gray]{this.PluginPath}[/]");
            try {
                string[] dlls = Directory.GetFiles(this.PluginPath, "*.dll");
                if (dlls.Length == 0) {
                    this.logger.Info("Es sind keine Plugins verfügbar");
                    return;
                }

                foreach (string dll in dlls) {
                    var ass = Assembly.LoadFrom(dll);
                    AddPlugin(ass);
                }
            }
            catch (DirectoryNotFoundException e) {
                this.logger.Error($"Es sieht so aus als würde es den Pfad nicht geben: {this.PluginPath}");
            }
            catch (ReflectionTypeLoadException ex) {
                this.logger.Error($"Fehler beim laden von Plugin.");
            }

            if (LoadedPlugins() > 0) {
                this.logger.Success($"Es wurden [bold green]{LoadedPlugins()}[/] Plugins geladen.");
            } else {
                this.logger.Info("Keine Plugins geladen.");
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
            this.logger.Info("Plugins werden neugestartet...");
            this.PluginsList.Clear();
            LoadPlugins();
            this.logger.Success("Neustart der Plugins erfolgreich :tada:");
        }

        private void OnInit(Plugin plugin) {
            try {
                this.logger.Info($"initialisiere Plugin [springgreen3]{plugin.Name}[/]");
                plugin.OnServerInit();
                this.logger.Success($"initialisierung von Plugin {plugin.Name} war erfolgreich");
                this.PluginsList.Add(plugin);
            }
            catch (Exception ex) {
                this.logger.Exception(ex);
                this.logger.Warning(
                    $"Fehler beim initialisieren von Plugin [bold springgreen3]{plugin.Name}[/]. [bold red]{ex.Message}[/]. System wird das Plugin [bold springgreen3]{plugin.Name}[/] ignorieren.");
            }
        }

        private void OnStartup() {
            foreach (var plugin in this.PluginsList) {
                try {
                    this.logger.Info($"Plugin {plugin.Name} wird gestartet");
                    plugin.OnServerStartup();
                    this.logger.Success($"Plugin [bold springgreen3]{plugin.Name}[/] wurde erfolgreich gestartet");
                }
                catch (Exception ex) {
                    this.logger.Exception(ex);
                    this.logger.Error($"Ein Fehler ist beim starten von Plugin {plugin.Name} aufgetreten. {ex.Message}");
                }
            }
        }

        public void OnStop() {
            foreach (var plugin in this.PluginsList) {
                this.logger.Info($"Plugin [gray]{plugin.Name}[/] wird abgemeldet.");
                plugin.OnServerStop();
            }
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
