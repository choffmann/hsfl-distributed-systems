using PluginContract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPlugin
{
    public class DemoPlugin : PluginContract.Plugin {

        public DemoPlugin(): base("DemoPlugin") { }

        public override void OnInit() {
            TestLogger();
            Thread.Sleep(2000);
            //throw new NotImplementedException("Ein Fehler beim Aufruf von OnInit() :smiling_face_with_horns:");
        }

        public override void OnStartup() {
            Thread.Sleep(2000);
        }

        private void TestLogger() {
            Logger.Debug($"Eine Debug Ausgabe vom Plugin :bug:");
            Logger.Info($"Eine Info Ausgabe vom Plugin :waving_hand:");
            Logger.Success($"Eine Erfolgreiche Ausgabe vom Plugin :check_mark_button:");
            Logger.SuccessEmoji($"Eine Erfolgreiche Ausgabe vom Plugin mit Emoji");
            Logger.SuccessEmoji(":otter:", "Eine Erfolgreiche Ausgabe vom Plugin mit eigenen Emoji");
            Logger.Warning($"Eine Warnung Ausgabe vom Plugin :warning:");
            Logger.Error($"Eine Fehler Ausgabe vom Plugin :cross_mark:");
            Logger.WriteToLogFile("DEBUG", "Eine Ausgabe nur in die Log-File vom Plugin");
        }
    }
}
