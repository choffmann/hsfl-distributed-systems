using System;
using System.ComponentModel;
using hsfl.ceho5518.vs.Client.Logger;
using Spectre.Console.Cli;

namespace hsfl.ceho5518.vs.Client.Commands {
    public class DefaultSettings : CommandSettings {
        [CommandOption("--logFile")]
        [Description("Path and file name for logging")]
        public string LogFile { get; set; }

        [CommandOption("-v|--verbose")]
        [Description("Enable verbose mode")]
        [DefaultValue(false)]
        public bool VerboseMode { get; set; }
    }

    public class LogInterceptor : ICommandInterceptor {
        public void Intercept(CommandContext context, CommandSettings settings) {
            SetLogger(settings);
        }


        private static void SetLogger(CommandSettings settings) {
            if (settings is DefaultSettings logSettings) {
                if (logSettings.VerboseMode) {
                    ClientLogger.Logger = LoggerService.Logger.Instance;
                } else {
                    ClientLogger.Logger = new NoLogger();
                }
            } else {
                ClientLogger.Logger = new NoLogger();
            }
        }
    }
}
