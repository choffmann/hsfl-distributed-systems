﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using PluginContract;
using PluginContract.ClientPlugin;

namespace Primfaktorzerlegung {
    public class PrimePlugin : Plugin {
        public override string CommandName { get; } = "prime";
        public int PrimeInput { get; set; }
        public string PrimeOutput { get; set; }
        public PrimePlugin() : base("Primfaktorzerlegung") { }

        public override void OnServerExecute(string[] args) {
            string input = args[0] ?? throw new ArgumentException("Pfad zur Datei wurde nicht angegeben");
            try {
                int inputNumber = Convert.ToInt32(input);
                var calc = Calc(inputNumber);
                string output = "";
                for (int i = 1; i < calc.Count; i++) {
                    if (i == calc.Count - 1) {
                        output += $"{calc[i]} = {inputNumber}";
                    } else {
                        output += $"{calc[i]} * ";
                    }
                }
                this.Logger.Debug("Warte 30 Sekunden zur Simulation. Status sollte 'WORKING' sein.");
                Thread.Sleep(30000);
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
