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
        private ILogger logger = hsfl.ceho5518.vs.LoggerService.Logger.Instance;
        private static PluginContract.Logger instance = new PluginContract.Logger();

        private Logger() { }

        public static Logger Instance { get { return instance; } }

        public void Debug(string message) {
            logger.Debug($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void Error(string message) {
            logger.Error($"[red]{PluginName}[/]: {message}");
        }

        public void Exception(Exception exception) {
            logger.Exception(exception);
        }

        public void Info(string message) {
            logger.Info($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void Success(string message) {
            logger.Success($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void SuccessEmoji(string message) {
            logger.SuccessEmoji($"[turquoise2]{PluginName}[/]: {message}");
        }

        public void SuccessEmoji(string emoji, string message) {
            logger.SuccessEmoji(emoji, $"[turquoise2]{PluginName}[/]: {message}");
        }

        public void Warning(string message) {
            logger.Warning($"[orange4]{PluginName}[/]: {message}");
        }

        public void WriteToLogFile(string level, string message) {
            logger.WriteToLogFile(level, $"{PluginName}: {message}");
        }
    }
}
