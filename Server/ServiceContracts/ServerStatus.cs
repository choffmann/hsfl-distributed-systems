namespace hsfl.ceho5518.vs.server.ServiceContracts {
    public enum ServerStatus {
        STARTING, IDLE, LOADING, WORKING, ERROR
    }

    public class ServerStatusDetail {
        public string Id { get; set; }
        public ServerStatus CurrentState { get; set; }
        public bool IsWorker { get; set; }
    }
}
