using PluginContract;

namespace Primfaktorzerlegung {
    public class PrimePlugin : Plugin {
        public PrimePlugin() : base("Primfaktorzerlegung") {}

        public override void OnInit() {
            this.Logger.Info("Wird gestartet...");
        }
    }
}
