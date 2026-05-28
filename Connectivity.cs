using Archipelago.Multiclient.Net;

namespace BiomorphRandomizer.Connectivity;

static class SessionTools {
	private static string host = "localhost";
	private static int port = 38281;
	private static string password = "";
	
	public static ArchipelagoSession Session;
	
	public static void CreateSession() {
		Session = ArchipelagoSessionFactory.CreateSession(host, port);
	}
}