using System.Collections.Generic;
using Il2CppLDS.MindBreaker.Core;
using Il2CppPixelCrushers.DialogueSystem;

namespace BiomorphRandomizer;

public static class LocationFinder {
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
	
	public static long IdFromSerializationData(string data) {
		if (locationDictionary.ContainsKey(data)) {
			return locationDictionary[data];
		} else {
			return -1;
		}
	}
	
	// I'll move the LocationsAlreadyFound functionality here
	// It doesn't need to have multiple instances of a separate class
	
	public static void CheckForGoal() {
		if (DialogueLua.GetVariable("Quest.MQ02_State0_Completed", false) && SessionTools.CheckConnection()) {
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
	
	private static List<long> excludedLocations = new List<long>(new long[] {306, 309, 14, 15, 16, 17, 18, 19});
	
	public static void FillLocationDictionary() {
		locationDictionary = new Dictionary<string, long>();
		
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
		
		// Mezzo Skyway
		locationDictionary.Add("SerializationData_Z04_Quest_Wrench_01", 401);
	}
}

/*
public class LocationsAlreadyFound {
	public List<long> LocationIds = new List<long>();
	
	public void CheckForLocations(GameData gameData) {
		if (DialogueLua.GetVariable("SerializationData_Z03_RawMaterial_01", false))
			LocationIds.Add(1);
		if (DialogueLua.GetVariable("SerializationData_Z03_Laurentium_01", false))
			LocationIds.Add(2);
		if (DialogueLua.GetVariable("SerializationData_Z03_VitalModule_01", false))
			LocationIds.Add(3);
		if (DialogueLua.GetVariable("SerializationData_Z03_LogicBlocks_01", false))
			LocationIds.Add(4);
		if (DialogueLua.GetVariable("SerializationData_Memento_02", false))
			LocationIds.Add(5);
		return;
	}
	
	public LocationsAlreadyFound(GameData gameData) {
		CheckForLocations(gameData);
	}
}
*/