using System;
using hsfl.ceho5518.vs.LoggerService;

namespace hsfl.ceho5518.vs.Client {
    public class NoLogger: ILogger {
        /**
         * Disable Logger and only write to log files
         */
        public LogLevel LogLevel { get; set; }
        public void Info(string message) {
            WriteToLogFile("INFO", message);
        }
        public void Success(string message) {
            WriteToLogFile("SUCCESS", message);
        }
        public void Warning(string message) {
            WriteToLogFile("WARNING", message);
        }
        public void Error(string message) {
            WriteToLogFile("ERROR", message);
        }
        public void Exception(Exception exception) {
            WriteToLogFile("EXCEPTION", exception.Message);
        }
        public void Debug(string message) {
            WriteToLogFile("DEBUG", message);
        }
        public void WriteToLogFile(string level, string message) {
            
        }
    }
}
