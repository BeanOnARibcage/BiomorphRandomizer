using System.Collections.Generic;
using Il2CppLDS.MindBreaker.Core;
using Il2CppPixelCrushers.DialogueSystem;
using Il2CppLDS.MindBreaker.Data;

namespace BiomorphRandomizer;

public static class LocationFinder {
	public static List<long> UnscoutedLocations = new List<long>();
	// for locations which were checked without the scout and got the generic "Archipelago Item"
	
	public static bool StartingWeaponFound() {
		return DialogueLua.GetVariable("SerializationData_Z03_Weapons", false);
	}
	
	public static long IntroIdFromInteractionName(string interaction_name) {
		if (interaction_name == "Prefab_Interaction_RawMaterials_Z03_01") {
			return 1;
		}
		if (interaction_name == "Prefab_Interaction_Laurentium") {
			return 2;
		}
		if (interaction_name == "Prefab_Interaction_VitalModule_01") {
			return 3;
		}
		if (interaction_name == "Prefab_Interaction_LogicBlocks_Z03_01") {
			return 4;
		}
		if (interaction_name == "Prefab_Interaction_Memento_02") {
			return 5;
		}
		return -1;
	}
	
	private static Dictionary<string, long> locationDictionary;
	private static Dictionary<long, string> reverseLocationDictionary;
	
	public static long IdFromSerializationData(string data) {
		if (locationDictionary.ContainsKey(data)) {
			return locationDictionary[data];
		} else {
			return -1;
		}
	}
	
	public static void CheckForGoal() {
		bool goalCondition = DialogueLua.GetVariable("SerializationData_Z01_Map_01", 0) > 0 ||
			DialogueLua.GetVariable("SerializationData_Z01_Map_01", 0) > 0;
		if (goalCondition && SessionTools.CheckConnection()) {
			SessionTools.SendGoal();
		}
	}
		
	public static void CheckForLocations() {
		foreach (KeyValuePair<string, long> pair in locationDictionary) {
			if (!excludedLocations.Contains(pair.Value) && DialogueLua.GetVariable(pair.Key, false)) {
				SessionTools.SendLocation(pair.Value);
			}
		}
	}
	
	public static bool IsLocationChecked(long id) {
		string serializationData = reverseLocationDictionary[id];
		return DialogueLua.GetVariable(serializationData, false);
	}
	
	public static bool IsLocationIncluded(long id) {
		return !excludedLocations.Contains(id);
	}
	
	public static ItemData ItemBeingFound(long id, bool itemIsBeingGranted) {
		ItemData result;
		if (SessionTools.LocationScouts != null) {
			Archipelago.MultiClient.Net.Models.ScoutedItemInfo itemInfo = SessionTools.LocationScouts[id];
			if (itemInfo.Player.Equals(SessionTools.ActivePlayer)) {
				result = ItemGiver.GetItemData(itemInfo.ItemId, itemIsBeingGranted);
			} else {
				result = UnityEngine.Object.Instantiate(ItemGiver.APItemData).Cast<ItemData>();
				result._NameID = itemInfo.ItemDisplayName;
				result._DescriptionID = "An item from another world. It belongs to " +
					itemInfo.Player.Name + " in " + itemInfo.ItemGame + ".";
			}
		} else if (ItemGiver.ItemsBeforeLocations.ContainsKey(id)) {
			result = ItemGiver.GetItemData(ItemGiver.ItemsBeforeLocations[id], itemIsBeingGranted);
		} else {
			result = ItemGiver.APItemData;
			if (itemIsBeingGranted) {
				LocationFinder.UnscoutedLocations.Add(id);
			}
		}
		return result;
	}
	
	public static void RecordLocationSerializationData(long id) {
		DialogueLua.SetVariable(reverseLocationDictionary[id], true);
	}
	
	public static string MonsterName(long id) {
		if (id >= 2000) {
			if (id <= 2002)
				return "Fubirang";
			else if (id <= 2005)
				return "Scarbyttle";
		}
		return null;
	}
	
	private static List<long> excludedLocations = new List<long>(new long[] {306, 309, 14, 15, 16, 17, 18, 19,
		20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45,
		46, 47, 48, 49, 50, 51, 55, 56, 57, 58, 403, 409, 412, 414, 415, 416, 2001, 2002, 2005});
	
	public static void FillLocationDictionary() {
		locationDictionary = new Dictionary<string, long>();
		reverseLocationDictionary = new Dictionary<long, string>();
		
		// Core's Lab
		locationDictionary.Add("SerializationData_Z03_Weapons", 301);
		locationDictionary.Add("SerializationData_Z03_RawMaterial_01", 302);
		locationDictionary.Add("SerializationData_Z03_Laurentium_01", 303);
		locationDictionary.Add("SerializationData_Z03_VitalModule_01", 304);
		locationDictionary.Add("SerializationData_Z03_LogicBlocks_01", 305);
		locationDictionary.Add("SerializationData_Z03_BiomorphPart", 306);
		locationDictionary.Add("SerializationData_Memento_02", 307);
		locationDictionary.Add("Items.Scargatos.22", 308);
		locationDictionary.Add("SerializationData_Z03_Section_1", 309);
		
		// Blightmoor
		locationDictionary.Add("Items.Scargatos.30", 1);
		locationDictionary.Add("Items.Scargatos.01", 2);
		locationDictionary.Add("ShopItemData_Chips_17_LittleHelper_01", 3);
		locationDictionary.Add("ShopItemData_Chips_01_FerroxField_01", 4);
		locationDictionary.Add("ShopItemData_Items_MementoSocket_SlarShop_01", 5);
		locationDictionary.Add("ShopItemData_Mementos_15_Phase3Accelerator_01", 6);
		locationDictionary.Add("ShopItemData_Mementos_03_CombatDiskette_01", 7);
		locationDictionary.Add("Quest.SQ02_State2_Completed", 8); // need to double-check the Boyd quest
		locationDictionary.Add("Quest.SQ02_State4_Completed", 9);
		locationDictionary.Add("Quest.SQ02_State5_Completed", 10);
		locationDictionary.Add("ShopItemData_Mementos_01_BloodColoredGlasses_01", 11);
		locationDictionary.Add("ShopItemData_Blueprint_BoydShop_02", 12);
		locationDictionary.Add("ShopItemData_Items_AttackModule_Asrar_01", 13);
		locationDictionary.Add("ShopItemData_Mementos_10_HeavyBelt", 14);
		locationDictionary.Add("ShopItemData_Items_VitalModule_Asrar_01", 15);
		locationDictionary.Add("ShopItemData_Blueprint_MarleShop_02", 16);
		locationDictionary.Add("ShopItemData_Mementos_11_EarpieceRadar", 17);
		locationDictionary.Add("ShopItemData_Blueprint_ChipImprinterShop_02", 18);
		locationDictionary.Add("ShopItemData_LogicBlocks_AsrarShop_01", 19);
		locationDictionary.Add("ShopItemData_Mementos_17_SapphireStainedGlass_01", 20);
		locationDictionary.Add("ShopItemData_LogicBlocks_AsrarShop_01", 21);
		locationDictionary.Add("ShopItemData_Items_EfficiencyModule_Asrar_01", 22);
		locationDictionary.Add("ShopItemData_LogicBlocks_SalmShop_01", 23);
		locationDictionary.Add("ShopItemData_Mementos_07_PositiveSolenoid_01", 24);
		locationDictionary.Add("ShopItemData_Chips_18_FerroxSpecter_01", 25);
		locationDictionary.Add("ShopItemData_Items_MementoSocket_SalmShop_01",26);
		locationDictionary.Add("ShopItemData_Mementos_04_DemodulatorImplant_01", 27);
		locationDictionary.Add("ShopItemData_Items_MementoSocket_SalmShop_02", 28);
		locationDictionary.Add("ShopItemData_LogicBlocks_SalmShop_02", 29);
		locationDictionary.Add("ShopItemData_Mementos_08_DoubleEdgedLocket_01", 30);
		locationDictionary.Add("ShopItemData_Items_MementoSocket_SalmShop_03", 31);
		locationDictionary.Add("ShopItemData_LogicBlocks_SalmShop_03", 32);
		locationDictionary.Add("ShopItemData_Blueprint_BoydShop_03", 33);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_01", 34);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_02", 35);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_03", 36);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_04", 37);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_05", 38);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_06", 39);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_07", 40);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_08", 41);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_09", 42);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_10", 43);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_11", 44);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_12", 45);
		locationDictionary.Add("ShopItemData_Collectible_CarmelinaShop_13", 46);
		locationDictionary.Add("SerializationData_Quest_SQ08_RawMaterial", 47);
		locationDictionary.Add("SerializationData_Quest_SQ08_Laurentium", 48);
		locationDictionary.Add("SerializationData_Quest_SQ08_LogicBlocks", 49);
		locationDictionary.Add("SerializationData_Quest_SQ08_Reward_27", 50);
		locationDictionary.Add("SerializationData_Quest_SQ08_Reward_39", 51);
		locationDictionary.Add("Quest.SQ20_State3_Completed", 52);
		locationDictionary.Add("Quest.SQ35_State3_Completed", 53);
		locationDictionary.Add("NPCs.Boyd_Shop2_Finished", 54);
		locationDictionary.Add("NPCs.Boyd_Shop3_Finished", 55);
		locationDictionary.Add("SerializationData_NPC_Will_Z00_01", 59);
		
		// Mezzo Skyway
		locationDictionary.Add("SerializationData_Z04_Quest_Wrench_01", 401);
		locationDictionary.Add("SerializationData_Z04_LogicBlocks_02", 402);
		locationDictionary.Add("SerializationData_Z04_Chest_03", 403);
		locationDictionary.Add("Items.Scargatos.11", 404);
		locationDictionary.Add("SerializationData_NPC_Will_Z04_06", 405);
		locationDictionary.Add("SerializationData_Blueprint_MarleShop_01", 406);
		locationDictionary.Add("SerializationData_Z04_Memento_18", 407);
		locationDictionary.Add("SerializationData_Z04_LogicBlocks_03", 408);
		locationDictionary.Add("SerializationData_Z04_Laurentium_01", 409);
		locationDictionary.Add("SerializationData_Quest_SQ35_LetterTapper_01", 410);
		locationDictionary.Add("SerializationData_Z04_RawMaterial_01", 411);
		locationDictionary.Add("SerializationData_Z04_Section_1", 412);
		locationDictionary.Add("SerializationData_Z04_Boss_MementoSocket_01", 413);
		locationDictionary.Add("SerializationData_Z04_RawMaterial_04", 414);
		locationDictionary.Add("Items.Scargatos.12", 415);
		locationDictionary.Add("SerializationData_Z04_Chest_02", 416);
		locationDictionary.Add("SerializationData_Z04_LogicBlocks_01", 417);
		locationDictionary.Add("SerializationData_Z04_RawMaterial_03", 418);
		
		// Biomorphs
		locationDictionary.Add("Archipelago_Fubirang_01", 2000);
		locationDictionary.Add("Archipelago_Fubirang_02", 2001);
		locationDictionary.Add("Archipelago_Fubirang_03", 2002);
		locationDictionary.Add("Archipelago_Scarbyttle_01", 2003);
		locationDictionary.Add("Archipelago_Scarbyttle_02", 2004);
		locationDictionary.Add("Archipelago_Scarbyttle_03", 2005);
		
		foreach (KeyValuePair<string, long> pair in locationDictionary) {
			reverseLocationDictionary.Add(pair.Value, pair.Key);
		}
	}
}