using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System.Threading.Tasks;
using MelonLoader;
using Archipelago.MultiClient.Net.Models;

namespace BiomorphRandomizer;

public static class SessionTools {
	private static string host;
	private static int port;
	private static string password;
	private static string slotname;
	
	public static ArchipelagoSession Session;
	
	public static void CreateSession() {
		host = Preferences.Host.Value;
		port = Preferences.Port.Value;
		Session = ArchipelagoSessionFactory.CreateSession(host, port);
	}
	
	public static void Connect() {
		LoginResult result;
		password = Preferences.Password.Value;
		slotname = Preferences.Slotname.Value;
		if (password == "") {
			password = null;
		}
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
	
	public static void SendLocation(int id) {
		Session.Locations.CompleteLocationChecks(id);
	}
	
	public static void SendGoal() {
		Session.SetGoalAchieved();
	}
	
	// returns true if an item was received, so we can immediately check for the next item
	public static bool CheckForAndReceiveItem() {
		if (CheckConnection() && Session.Items.Any()) {
			ItemInfo item;
			item = Session.Items.DequeueItem();
			Melon<Randomizer>.Logger.Msg("Dequeued item " + item.ItemDisplayName);
			ItemGiver.IntroGiveItemFromId((int)item.ItemId);
			return true;
		}
		return false;
	}
}