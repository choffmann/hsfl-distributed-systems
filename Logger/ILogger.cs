using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hsfl.ceho5518.vs.LoggerService {
    public interface ILogger {
        LogLevel LogLevel { get; set; }
        void Info(string message);
        void Success(string message);
        void Warning(string message);
        void Error(string message);
        void Exception(Exception exception);
        void Debug(string message);
        void WriteToLogFile(string level, string message);
    }
}
