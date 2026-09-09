using Il2CppLDS.Framework.Inventory;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.MindBreaker.Data;
using UnityEngine;
using MelonLoader;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Reflection;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Il2CppLDS.Sardonyx.Actions;
using System.Collections.Generic;
using Il2CppLDS.Framework.Core;

namespace BiomorphRandomizer;

public static class ItemGiver {
	public static Scene APScene;
	public static GameObject APPickItemGO = null;
	public static InteractionPickItem APPickItem = null;
	public static ItemData APItemData = null;
	// use APPickItem for receiving items (subbing in the corresponding ItemData)
	// use APItemData for sending locations (putting it into the game's existing InteractionPickItem)
	
	public static bool BruisersReceived = false;
	public static Dictionary<long, long> ItemsBeforeLocations = new Dictionary<long, long>();
	// for item packets with a local location that hasn't been checked in the game yet
	// key is location ID and value is item ID
	
	public static bool CanGetItem() {
		return GameManager.CanUseInteractions && APPickItem != null;
	}
	
	public static void GiveChip(Chips chip) {
		WeaponData chipData = InventoryHandler.ItemDatabase.Chips[(int)chip];
		InventoryHandler.UpdateItem((ItemData)chipData, 1);
	}
	
	public static void IntroGiveItemFromId(int id) {
		if (id < 1 || id > 5) {
			return;
		}
		ItemData item = null;
		if (id == 1) {
			item = InventoryHandler.ItemDatabase.RawMaterials;
		}
		if (id == 2) {
			item = InventoryHandler.ItemDatabase.Laurentium;
		}
		if (id == 3) {
			item = InventoryHandler.ItemDatabase.VitalModules;
		}
		if (id == 4) {
			item = InventoryHandler.ItemDatabase.LogicBlocks;
		}
		if (id == 5) {
			item = (ItemData)InventoryHandler.ItemDatabase.MementoHorseshoeMagnet;
		}
		if (item != null) {
			InventoryHandler.UpdateItem(item, 1);
		}
		return;
	}
	
	public static void GiveItemFromId(long id, bool local, long locationId) {
		// Clone the saved interaction because they're not reusable
		InteractionPickItem interaction = UnityEngine.Object.Instantiate(APPickItem, APScene).Cast<InteractionPickItem>();
		interaction._ItemQuantity = 1;
		if (id == 418) {
			BruisersReceived = true;
			if (!LocationFinder.StartingWeaponFound()) {
				interaction._ItemQuantity = 0;
			} // Having the bruisers before entering Z03_03 from the lower left locks you behind a door
		}
		if (id == (long)SessionTools.SlotData["starting_weapon"]) {
			return; // Starting weapon is always handled locally
		}
		if (local && !LocationFinder.UnscoutedLocations.Contains(locationId)) {
			return; // location was scouted and handled locally
		}
		interaction._ItemData = GetItemData(id);
		interaction.ExecutePickItem();
	}
	
	public static int FindItemCount(string variables) {
		int index = variables.IndexOf("ArchipelagoItems");
		if (index == -1)
			return 0;
		else {
			if (int.TryParse(variables.Substring(index + 17, 3), out int number))
				return number;
			else
				return 0;
		}
	}
	
	private static void log(string message) {
		Melon<Randomizer>.Logger.Msg(message);
	}
	
	public static void MakeAPInteraction() {
		APScene = SceneManager.GetSceneByName("Master");
		APPickItemGO = new GameObject("Archipelago");
		SceneManager.MoveGameObjectToScene(APPickItemGO, APScene);
		APPickItem = APPickItemGO.AddComponent<InteractionPickItem>();
		APPickItem._ShowNotification = true;
		APItemData = UnityEngine.Object.Instantiate(InventoryHandler.ItemDatabase.Laptops).Cast<ItemData>();
		APPickItem._ItemData = APItemData;
		APItemData._NameID = "Randomized Item";
		APItemData._DescriptionID = "A randomized item from Archipelago.";
	}
	
	public static ItemData GetItemData(long id) {
		return GetItemData(id, true);
	}
	
	public static ItemData GetItemData(long id, bool itemIsBeingGranted) {
		if (itemIsBeingGranted) {
			Quests.CheckForQuestItem(id);
		}
		int ones, hundreds;
		ItemDatabase items = InventoryHandler.ItemDatabase;
		hundreds = Math.DivRem((int)id, 100, out ones);
		switch (hundreds) {
			case 1: // Abilities
				return items.Abilities[ones];
			case 3: // Blueprints
				return items.Blueprints[ones].Cast<ItemData>();
			case 4: // Chips
				return items.Chips[ones];
			case 5: // Mementos
				return items.Mementos[ones];
			case 6: // Scargatos
				return items.Scargatos[ones];
			case 8: // Other key items
				return items.KeyItems[ones];
			case 0: // Other
				if (ones == 1)
					return items.Laptops.Cast<ItemData>();
				else if (ones == 2)
					return items.MementoSocket;
				else
					return null;
			default:
				return null;
		}
	}
}

public enum Chips {
	FerroxField = 0,
	CalamityBringer = 3,
	ScargatoWrath = 6,
	Executioner = 9,
	ShadowSting = 12,
	PainLegion = 15,
	Bruisers = 18,
	BarbDrizzle = 21,
	ForsakenEater = 24,
	PhoenixBlow = 27,
	LittleHelper = 30,
	FerroxSpecter = 33,
	FerroxRoar = 36,
	MechaCrash = 39,
	FerroxHurricane = 42
};