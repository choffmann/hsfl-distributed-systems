using hsfl.ceho5518.vs.LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginContract {
    public class Logger : ILogger {
        public LogLevel LogLevel { get; set; }
        public string PluginName { get; set; }
        private readonly ILogger logger = hsfl.ceho5518.vs.LoggerService.Logger.Instance;

        private Logger() { }

        public static Logger Instance { get; } = new PluginContract.Logger();

        public void Debug(string message) {
            this.logger.Debug($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void Error(string message) {
            this.logger.Error($"[red]{PluginName}[/]: {message}");
        }

        public void Exception(Exception exception) {
            this.logger.Exception(exception);
        }

        public void Info(string message) {
            this.logger.Info($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void Success(string message) {
            this.logger.Success($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void SuccessEmoji(string message) {
            this.logger.SuccessEmoji($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void SuccessEmoji(string emoji, string message) {
            this.logger.SuccessEmoji(emoji, $"[turquoise2]{PluginName}[/]: {message}");
        }

        public void Warning(string message) {
            this.logger.Warning($"[orange4]{PluginName}[/]: {message}");
        }

        public void WriteToLogFile(string level, string message) {
            this.logger.WriteToLogFile(level, $"{PluginName}: {message}");
        }
    }
}
