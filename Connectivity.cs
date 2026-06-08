using Archipelago.Multiclient.Net;

namespace BiomorphRandomizer;

static class SessionTools {
	private static string host = "localhost";
	private static int port = 38281;
	private static string password = "";
	
	public static ArchipelagoSession Session;
	
	public static void CreateSession() {
		Session = ArchipelagoSessionFactory.CreateSession(host, port);
	}
	
	public static void Connect(string slotname) {
		LoginResult result;
		result = Session.TryConnectAndLogin("Biomorph", slotname, ItemsHandlingFlags.AllItems);
		if (result.Successful) {
			return;
		}
		else {
			return;
		}
	}
	
	public static bool CheckConnection() {
		return Session.ConnectionInfo.Slot > -1;
	}
}