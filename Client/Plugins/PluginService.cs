using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContract.ClientPlugin;
using Spectre.Console;
using Plugin = PluginContract.Plugin;


namespace hsfl.ceho5518.vs.Client.Plugins {
    public class PluginService {
        private static PluginService instance;
       // private readonly List<PluginContract.Plugin> pluginsList = new List<PluginContract.Plugin>();
        public List<Plugin> PluginList { get; } = new List<Plugin>();
        private readonly string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

        private PluginService() {
            LoadOnStart();
        }

        public static PluginService GetInstance() {
            return instance ?? (instance = new PluginService());
        }

        public void LoadOnStart() {
            AnsiConsole.Status().Spinner(Spinner.Known.Moon).Start("Register Plugins...", ctx => {
                CreatePluginFolder();
                LoadPlugins();
                OnStartup();
            });
        }

        private void CreatePluginFolder() {
            if (!Directory.Exists(this.pluginPath)) {
                Directory.CreateDirectory(this.pluginPath);
            }
        }

        private void LoadPlugins() {
            try {
                string[] dlls = Directory.GetFiles(this.pluginPath, "*.dll");
                if (dlls.Length == 0) return;
                foreach (string dll in dlls) {
                    var ass = Assembly.LoadFrom(dll);
                    AddPlugin(ass);
                }
            }
            catch (DirectoryNotFoundException e) {
                AnsiConsole.WriteException(e);
            }
            catch (ReflectionTypeLoadException e) {
                AnsiConsole.WriteException(e);
            }
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
        
        private void OnInit(Plugin plugin) {
            try {
                AnsiConsole.MarkupLine($"Register plugin [springgreen3]{plugin.Name}[/]");
                plugin.OnClientInit();
                AnsiConsole.MarkupLine($"Successfully register plugin {plugin.Name}");
                this.PluginList.Add(plugin);
            }
            catch (Exception ex) {
                AnsiConsole.WriteException(ex);
            }
        }
        
        private void OnStartup() {
            foreach (var plugin in this.PluginList) {
                try {
                    AnsiConsole.MarkupLine($"Start Plugin {plugin.Name}");
                    plugin.OnClientStart();
                }
                catch (Exception ex) {
                    AnsiConsole.WriteException(ex);
                }
            }
        }
    }
}
