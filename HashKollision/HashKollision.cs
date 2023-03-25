using System.IO;
using System.Security.Cryptography;
using PluginContract;

namespace HashKollision {
    public class HashKollision : Plugin {
        public override string CommandName { get; } = "hash";

        public HashKollision() : base("HashKollision") { }
        public override void OnServerExecute(string[] input) {
            string file = input[0];

            byte[] inputBytes = File.ReadAllBytes(file);
            SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

        }
    }
}
