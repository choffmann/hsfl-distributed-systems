using System.Collections.Generic;
using System.Dynamic;

namespace hsfl.ceho5518.vs.ServiceContracts.Observer {
    public interface IPluginObserver {
        void OnPluginUpload(PluginObserver plugin);
    }

    public class PluginObserver {
        private static PluginObserver instance;
        private List<IPluginObserver> _observers = new List<IPluginObserver>();
        private byte[] _assembly;

        public byte[] Assembly {
            get {
                return this._assembly;
            }
            set {
                this._assembly = value;
                NotifyObservers();
            }
        }

        private PluginObserver() {
        }

        public static PluginObserver GetInstance() {
            if (instance == null) {
                instance = new PluginObserver();
            }
            return instance;
        }
        
        public void AddObserver(IPluginObserver observer) {
            this._observers.Add(observer);
        }

        public void RemoveObserver(IPluginObserver observer) {
            this._observers.Remove(observer);
        }

        private void NotifyObservers() {
            foreach (var observer in this._observers) {
                observer.OnPluginUpload(this);
            }
        }
    }
}
