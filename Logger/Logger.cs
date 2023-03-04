using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.LoggerService {
    public sealed class Logger : ILogger {
        private int levelValue = 0;
        private LogLevel level;

        private Logger() { }

        public static Logger Instance { get; } = new Logger();

        public LogLevel LogLevel {
            get { return this.level; }
            set {
                switch (value) {
                    case LogLevel.Debug:
                        this.levelValue = 0;
                        this.level = LogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        this.levelValue = 1;
                        this.level = LogLevel.Info;
                        break;
                    case LogLevel.Success:
                        this.levelValue = 2;
                        this.level = LogLevel.Success;
                        break;
                    case LogLevel.Warning:
                        this.levelValue = 3;
                        this.level = LogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        this.levelValue = 4;
                        this.level = LogLevel.Error;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public void Info(string message) {
            if (this.levelValue > 1)
                return;
            AnsiConsole.MarkupLine($"[gray][[INFO]][/] {message}");
            WriteToLogFile("INFO", message);
        }

        public void Success(string message) {
            if (this.levelValue > 1)
                return;
            AnsiConsole.MarkupLine($"[gray][[INFO]][/] [green]{message}[/]");
            WriteToLogFile("SUCCESS", message);
        }

        public void SuccessEmoji(string message) {
            if (this.levelValue > 2)
                return;
            AnsiConsole.MarkupLine($"[gray][[INFO]][/] :party_popper: [green]{message}[/]");
            WriteToLogFile("SUCCESS", message);
        }

        public void SuccessEmoji(string emoji, string message) {
            if (this.levelValue > 2)
                return;
            AnsiConsole.MarkupLine($"[gray][[INFO]][/] {emoji} [green]{message}[/]");
            WriteToLogFile("SUCCESS", message);
        }

        public void Warning(string message) {
            if (this.levelValue > 3)
                return;
            AnsiConsole.MarkupLine($"[orange4][[WARNING]][/] {message}");
            WriteToLogFile("WARNING", message);
        }

        public void Error(string message) {
            if (this.levelValue > 4)
                return;
            AnsiConsole.MarkupLine($"[red][[ERROR]][/] {message}");
            WriteToLogFile("ERROR", message);
        }

        public void Exception(Exception exception) {
            if (this.levelValue > 0)
                return;
            AnsiConsole.WriteException(exception);
            WriteToLogFile("EXCEPTION", exception.ToString());
        }

        public void Debug(string message) {
            if (this.levelValue > 0)
                return;
            AnsiConsole.MarkupLine($"[grey][[DEBUG]][/] {message}");
            WriteToLogFile("DEBUG", message);
        }

        public void WriteToLogFile(string level, string message) {
            Trace.WriteLine($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} - [{level}] {RemoveMarkup(message)}");
        }

        private string RemoveMarkup(string markup) {
            return Regex.Replace(markup, @"\[(?!\[)(.*?)\]", string.Empty);
        }
    }
}
