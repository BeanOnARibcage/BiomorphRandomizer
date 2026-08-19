using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System.Threading.Tasks;
using MelonLoader;

namespace BiomorphRandomizer;

public static class SessionTools {
	private static string host = "localhost";
	private static int port = 38281;
	private static string password = null;
	
	public static ArchipelagoSession Session;
	
	public static void CreateSession() {
		Session = ArchipelagoSessionFactory.CreateSession(host, port);
	}
	
	public static void Connect(string slotname) {
		LoginResult result;
		result = Session.TryConnectAndLogin("Biomorph", slotname, ItemsHandlingFlags.AllItems,
			password: password);
		if (result.Successful) {
			Melon<Randomizer>.Logger.Msg("Connection successful");
		}
		else {
			Melon<Randomizer>.Logger.Msg("Connection unsuccessful");
		}
		return;
	}
	
	public static bool CheckConnection() {
		return Session.ConnectionInfo.Slot > -1;
	}
}