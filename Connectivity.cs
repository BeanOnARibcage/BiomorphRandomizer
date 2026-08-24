using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System.Threading.Tasks;
using MelonLoader;
using Archipelago.MultiClient.Net.Models;
using System.Collections.Generic;

namespace BiomorphRandomizer;

public static class SessionTools {
	private static string host;
	private static int port;
	private static string password;
	private static string slotname;
	private static int itemsDequeued = 0;
	
	public static ArchipelagoSession Session;
	public static bool ReceivingItemsOkay = false;
	public static int ItemsProcessed = 0;
	
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
	
	public static void SendLocation(long id) {
		Session.Locations.CompleteLocationChecks(id);
	}
	
	public static void SendMultipleLocations(List<long> ids) {
		Session.Locations.CompleteLocationChecks(ids.ToArray());
	}
	
	public static void SendGoal() {
		Session.SetGoalAchieved();
	}
	
	// returns true if an item was dequeued, so we can immediately check for the next item
	public static bool CheckForAndReceiveItem() {
		if (CheckConnection() && Session.Items.Any()) {
			ItemInfo item;
			if (ItemsProcessed > itemsDequeued) {
				Session.Items.DequeueItem();
				itemsDequeued++;
			}
			else {
				item = Session.Items.DequeueItem();
				itemsDequeued++;
				ItemGiver.IntroGiveItemFromId((int)item.ItemId);
				ItemsProcessed++;
			}
			return true;
		}
		return false;
	}
}