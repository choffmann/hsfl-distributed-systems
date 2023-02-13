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
        private static Logger instance = new Logger();
        private int _levelValue = 0;
        private LogLevel _level;

        private Logger() { }

        public static Logger Instance { get { return instance; } }

        public LogLevel LogLevel {
            get { return _level; }
            set {
                switch (value) {
                    case LogLevel.Debug:
                        _levelValue = 0;
                        _level = LogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        _levelValue = 1;
                        _level = LogLevel.Info;
                        break;
                    case LogLevel.Success:
                        _levelValue = 2;
                        _level = LogLevel.Success;
                        break;
                    case LogLevel.Warning:
                        _levelValue = 3;
                        _level = LogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        _levelValue = 4;
                        _level = LogLevel.Error;
                        break;
                }
            }
        }

        public void Info(string message) {
            if (_levelValue <= 1) {
                AnsiConsole.MarkupLine($"[gray][[INFO]][/] {message}");
                WriteToLogFile("INFO", message);
            }
        }

        public void Success(string message) {
            if (_levelValue <= 1) {
                AnsiConsole.MarkupLine($"[gray][[INFO]][/] [green]{message}[/]");
                WriteToLogFile("SUCCESS", message);
            }
        }

        public void SuccessEmoji(string message) {
            if (_levelValue <= 2) {
                AnsiConsole.MarkupLine($"[gray][[INFO]][/] :party_popper: [green]{message}[/]");
                WriteToLogFile("SUCCESS", message);
            }
        }

        public void SuccessEmoji(string emoji, string message) {
            if (_levelValue <= 2) {
                AnsiConsole.MarkupLine($"[gray][[INFO]][/] {emoji} [green]{message}[/]");
                WriteToLogFile("SUCCESS", message);
            }
        }

        public void Warning(string message) {
            if (_levelValue <= 3) {
                AnsiConsole.MarkupLine($"[orange4][[WARNING]][/] {message}");
                WriteToLogFile("WARNING", message);
            }
        }

        public void Error(string message) {
            if (_levelValue <= 4) {
                AnsiConsole.MarkupLine($"[red][[ERROR]][/] {message}");
                WriteToLogFile("ERROR", message);
            }
        }

        public void Exception(Exception exception) {
            if (_levelValue <= 0) {
                AnsiConsole.WriteException(exception);
                WriteToLogFile("EXCEPTION", exception.ToString());
            }
        }

        public void Debug(string message) {
            if (_levelValue <= 0) {
                AnsiConsole.MarkupLine($"[grey][[DEBUG]][/] {message}");
                WriteToLogFile("DEBUG", message);
            }
        }

        public void WriteToLogFile(string level, string message) {
            Trace.WriteLine(string.Format("{0} - [{1}] {2}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), level, RemoveMarkup(message)));
        }

        private string RemoveMarkup(string markup) {
            return Regex.Replace(markup, @"\[(?!\[)(.*?)\]", string.Empty);
        }
    }
}
