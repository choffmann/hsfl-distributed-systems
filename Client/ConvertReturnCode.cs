namespace hsfl.ceho5518.vs.Client {
    public class ConvertReturnCode {
        public static string Convert(int code) {
            switch (code) {
                case 0:
                    return "[bold green]Serve:[/] OK";
                case 10:
                    return "[bold red]Server:[/] Aktuell sind keine Worker verfügbar";
                case 20:
                    return "[bold red]Server:[/] Der CommandName wurde nicht gefunden";
                case 28:
                    return "[bold red]Server:[/] Unerwarteter Fehler beim Registrieren des Plugins";
                case 29:
                    return "[bold red]Server:[/] Unerwarteter Fehler beim Ausführen des Plugins";
            }
            return "";
        }
    }
}
