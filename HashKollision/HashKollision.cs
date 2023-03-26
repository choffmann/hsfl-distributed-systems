using System;
using System.IO;
using System.Security.Cryptography;
using PluginContract;

namespace HashKollision {
    public class HashKollision : Plugin {
        public override string CommandName { get; } = "hash";
        public int MaxTimeOutMin = 5;

        public HashKollision() : base("HashKollision") { }
        public override void OnServerExecute(string[] input) {
            string filePath = input[0] ?? throw new ArgumentException("Pfad zur Datei wurde nicht angegeben");

            if (input.Length >= 2) {
                this.Logger.Debug($"Setze TimeOut auf {input[1]} Minuten.");
                this.MaxTimeOutMin = Convert.ToInt32(input[1]);
            }

            byte[] inputBytes = File.ReadAllBytes(filePath);
            var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
            this.Logger.Info($"Hash Wert der Datei ist: {hashString}");
            this.Logger.Info($"Starte die Hashwert Kontrolle");

            
            var timeOut = TimeSpan.FromMinutes(this.MaxTimeOutMin);
            var startTime = DateTime.Now;

            while(true) {
                if (DateTime.Now - startTime > timeOut) {
                    this.Logger.Info("TimeOut erreicht. Es wurde kein neuer Datensatz mit dem gleichen Hash Wert gefunden.");
                    break;
                }

                var random = new Random();
                inputBytes[random.Next(inputBytes.Length)] ^= (byte)random.Next(256);

                byte[] newHash = sha256.ComputeHash(inputBytes);
                string newHashString = BitConverter.ToString(newHash);
                this.Logger.Debug(newHashString);

                if (newHashString.Equals(hashString)) {
                    this.Logger.Success("Ein neuer Datensatz mit dem gleichen Hash Wert wurde gefunden!");
                    this.Logger.Info($"Wert: {BitConverter.ToString(inputBytes).Replace("-", "")}");
                    break;
                }
            }
        }
    }
}
