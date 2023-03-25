using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using PluginContract;
using PluginContract.ClientPlugin;

namespace Primfaktorzerlegung {
    public class PrimePlugin : Plugin {
        public override CommandArgument CommandArgument { get; set; } = new CommandArgument(0, "<PATH>");
        public override string CommandName { get; } = "prime";
        public override object CommandArgumentType { get; set; } = typeof(string);
        public int PrimeInput { get; set; }
        public string PrimeOutput { get; set; }
        public PrimePlugin() : base("Primfaktorzerlegung") { }

        public override void OnServerExecute(string input) {
            try {
                var calc = Calc(Convert.ToInt32(input));
                string output = "";
                for (int i = 1; i < calc.Count; i++) {
                    if (i == calc.Count - 1) {
                        output += $"{calc[i]} = {input}";
                    } else {
                        output += $"{calc[i]} * ";
                    }
                }
                this.Logger.Debug("Warte 1 Minute zur Simulation. Status sollte 'WORKING' sein.");
                Thread.Sleep(60000);
                this.Logger.Info($"Ergebnis ist: {output}");
            }
            catch (OverflowException e) {
                this.Logger.Error($"Fehler beim konvertieren der Nummer {input}");
                throw;
            }
        }
        public override void OnServerInit() {
            this.Logger.Info("Wird gestartet...");
        }


        /*public override void OnClientExecute(Action<int> number) {
            string output = "";
            var calc = Calc(number);
            for (int i = 1; i <= calc.Count; i++) {
                if (i == calc.Count) {
                    output += $"{i} = {number}";
                } else {
                    output += $"{i} * ";
                }
            }
            this.Logger.Info($"Ergebnis ist: {output}");
        }*/


        private static List<int> Calc(int number) {
            List<int> list = new List<int>();
            int n = number;
            int i = 2;
            while(i <= number && n > 1) {
                if (n % i == 0) {
                    list.Add(i);
                    n /= i;
                    i = 2;
                } else {
                    i++;
                }
            }
            return list;
        }
    }
}
