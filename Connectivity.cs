using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System.Threading.Tasks;
using MelonLoader;
using Archipelago.MultiClient.Net.Models;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Helpers;

namespace BiomorphRandomizer;

public static class SessionTools {
	private static string host;
	private static int port;
	private static string password;
	private static string slotname;
	private static int itemsDequeued = 0;
	
	public static ArchipelagoSession Session;
	public static int ItemsProcessed = 0;
	
	public static Dictionary<string, object> SlotData = null;
	public static Dictionary<long, ScoutedItemInfo> LocationScouts = null;
	public static Task<Dictionary<long, ScoutedItemInfo>> ScoutTask = null;
	
	public static void CreateSession() {
		host = Preferences.Host.Value;
		port = Preferences.Port.Value;
		Session = ArchipelagoSessionFactory.CreateSession(host, port);
	}
	
	public static void Connect() {
		if (timeOfLastConnectionAttempt < 0) {
			timeOfLastConnectionAttempt = 1;
		}
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
			ItemGiver.BruisersReceived = false;
			SlotData = Session.DataStorage.GetSlotData();
			ScoutTask = Session.Locations.ScoutLocationsAsync(
				HintCreationPolicy.None, Session.Locations.AllLocations.ToArray());
		}
		else {
			Melon<Randomizer>.Logger.Msg("Connection unsuccessful");
		}
		return;
	}
	
	public static void ReceiveLocationScouts() {
		if (ScoutTask.IsCompleted) {
			if (ScoutTask.IsCompletedSuccessfully) {
				LocationScouts = ScoutTask.Result;
			} else {
				Melon<Randomizer>.Logger.Msg("Scouts not received successfully");
				Melon<Randomizer>.Logger.Msg("Status: " + ScoutTask.Status.ToString());
				ScoutTask = null;
			}
		}
	}
			
	private static float timeOfLastConnectionAttempt = 0;
	private static Task reconnectionTask = null;
	public static bool FirstConnectionAttempt = false;
	
	public static void Reconnect() {
		if (!FirstConnectionAttempt) {
			return;
		}
		if (reconnectionTask == null) {
			if (UnityEngine.Time.fixedUnscaledTime > timeOfLastConnectionAttempt + 30) {
				reconnectionTask = Task.Run(Connect);
			}
		} else if (reconnectionTask.IsCompleted) {
			timeOfLastConnectionAttempt = UnityEngine.Time.fixedUnscaledTime;
			reconnectionTask = null;
		}
		LocationFinder.CheckForLocations();
	}
	
	public static bool CheckConnection() {
		return Session.ConnectionInfo.Slot > -1;
	}
	
	public static PlayerInfo ActivePlayer { 
		get {
			return Session.Players.ActivePlayer;
		}
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
	
	// returns true if we should immediately check for the next item (i.e. if money was dequeued)
	public static bool CheckForAndReceiveItem() {
		if (CheckConnection() && Session.Items.Any() && ItemGiver.CanGetItem() 
			&& ItemGiver.APPickItem != null && SlotData != null) {
			ItemInfo item;
			if (ItemsProcessed > itemsDequeued) {
				item = Session.Items.DequeueItem();
				if (item.ItemId == 418) {
					ItemGiver.BruisersReceived = true;
				}
				itemsDequeued++;
			}
			else {
				item = Session.Items.DequeueItem();
				itemsDequeued++;
				bool local = item.Player.Equals(ActivePlayer);
				if (local && !LocationFinder.IsLocationChecked(item.LocationId)) {
					ItemGiver.ItemsBeforeLocations.Add(item.LocationId, item.ItemId);
				}
				ItemGiver.GiveItemFromId(item.ItemId, local, item.LocationId);
				ItemsProcessed++;
			}
			return false;
		}
		return false;
	}
}