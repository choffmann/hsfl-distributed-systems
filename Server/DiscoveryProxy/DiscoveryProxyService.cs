using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using Spectre.Console;
using System.Xml;
using hsfl.ceho5518.vs.LoggerService;

namespace hsfl.ceho5518.vs.server.DiscoveryProxy {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DiscoveryProxyService : System.ServiceModel.Discovery.DiscoveryProxy {
        private readonly ILogger logger = Logger.Instance;
        readonly Dictionary<EndpointAddress, EndpointDiscoveryMetadata> onlineServices;

        public DiscoveryProxyService() {
            this.onlineServices = new Dictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }

        // The following are helper methods required by the Proxy implementation
        private void AddOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata) {
            lock (this.onlineServices) {
                this.onlineServices[endpointDiscoveryMetadata.Address] = endpointDiscoveryMetadata;
            }
            PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Adding");
        }

        private void RemoveOnlineService(EndpointDiscoveryMetadata endpointDiscoveryMetadata) {
            if (endpointDiscoveryMetadata != null) {
                lock (this.onlineServices) {
                    this.onlineServices.Remove(endpointDiscoveryMetadata.Address);
                }
            }
            PrintDiscoveryMetadata(endpointDiscoveryMetadata, "Removing");
        }

        private void MatchFromOnlineService(FindRequestContext findRequestContext) {
            lock (this.onlineServices) {
                foreach (var endpointDiscoveryMetadata in this.onlineServices.Values.Where(endpointDiscoveryMetadata => findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))) {
                    findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                }
            }
        }

        private EndpointDiscoveryMetadata MatchFromOnlineService(ResolveCriteria criteria) {
            EndpointDiscoveryMetadata matchingEndpoint = null;
            lock (this.onlineServices) {
                foreach (var endpointDiscoveryMetadata in this.onlineServices.Values.Where(endpointDiscoveryMetadata => criteria.Address == endpointDiscoveryMetadata.Address)) {
                    matchingEndpoint = endpointDiscoveryMetadata;
                }
            }
            return matchingEndpoint;
        }

        private void PrintDiscoveryMetadata(EndpointDiscoveryMetadata endpointDiscoveryMetadata, string verb) {
            this.logger.Info($"[bold]{verb}[/] service of the following type from cache.");
            foreach (var contractName in endpointDiscoveryMetadata.ContractTypeNames) {
                this.logger.Debug($"Contract Name: {contractName}");
                break;
            }
            this.logger.Success("Operation Completed");
        }

        private sealed class OnOnlineAnnouncementAsyncResult : AsyncResult {
            public OnOnlineAnnouncementAsyncResult(AsyncCallback callback, object state) : base(callback, state) {
                Complete(true);
            }
            public static void End(IAsyncResult result) {
                AsyncResult.End<OnOnlineAnnouncementAsyncResult>(result);
            }
        }

        private sealed class OnOfflineAnnouncementAsyncResult : AsyncResult {
            public OnOfflineAnnouncementAsyncResult(AsyncCallback callback, object state)
                : base(callback, state) {
                Complete(true);
            }

            public static void End(IAsyncResult result) {
                AsyncResult.End<OnOfflineAnnouncementAsyncResult>(result);
            }
        }

        private sealed class OnFindAsyncResult : AsyncResult {
            public OnFindAsyncResult(AsyncCallback callback, object state)
                : base(callback, state) {
                Complete(true);
            }

            public static void End(IAsyncResult result) {
                AsyncResult.End<OnFindAsyncResult>(result);
            }
        }

        private sealed class OnResolveAsyncResult : AsyncResult {
            private readonly EndpointDiscoveryMetadata matchingEndpoint;

            public OnResolveAsyncResult(EndpointDiscoveryMetadata matchingEndpoint, AsyncCallback callback, object state)
                : base(callback, state) {
                this.matchingEndpoint = matchingEndpoint;
                Complete(true);
            }

            public static EndpointDiscoveryMetadata End(IAsyncResult result) {
                var thisPtr = AsyncResult.End<OnResolveAsyncResult>(result);
                return thisPtr.matchingEndpoint;
            }
        }

        // OnBeginOnlineAnnouncement method is called when a Hello message is received by the Proxy
        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state) {
            AddOnlineService(endpointDiscoveryMetadata);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOnlineAnnouncement(IAsyncResult result) {
            OnOnlineAnnouncementAsyncResult.End(result);
        }

        // OnBeginOfflineAnnouncement method is called when a Bye message is received by the Proxy
        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state) {
            RemoveOnlineService(endpointDiscoveryMetadata);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOfflineAnnouncement(IAsyncResult result) {
            OnOfflineAnnouncementAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Probe request message is received by the Proxy
        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state) {
            MatchFromOnlineService(findRequestContext);
            return new OnFindAsyncResult(callback, state);
        }

        protected override void OnEndFind(IAsyncResult result) {
            OnFindAsyncResult.End(result);
        }

        // OnBeginFind method is called when a Resolve request message is received by the Proxy
        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state) {
            return new OnResolveAsyncResult(this.MatchFromOnlineService(resolveCriteria), callback, state);
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result) {
            return OnResolveAsyncResult.End(result);
        }
    }
}
