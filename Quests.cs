using Il2CppLDS.MindBreaker.Cinematics;
using Il2CppPixelCrushers.DialogueSystem;
using Il2CppLDS.MindBreaker.Data;

namespace BiomorphRandomizer;

public static class Quests {
	
	public static void SetQuestVariables(string map) {
		switch (map) {
			case "Z00_02":
				// Boyd's quest
				DialogueLua.SetVariable("Quest.SQ02_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ02_State1_Item", false));
				goto case "Z03_06";
			case "Z04_01":
				DialogueLua.SetVariable("Quest.SQ02_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ02_State1_Location", false));
				return;
			case "Z03_06":
				DialogueLua.SetVariable("Global.SAFEUpgradeUnlocked",
					DialogueLua.GetVariable("Quest.SQ02_State4_Completed", false) &&
					DialogueLua.GetVariable("Global.SAFEKits", 0) > 0);
				return;
		}
	}
	
	public static void CheckForQuestItem(long id) {
		switch (id) {
			case 820:
				DialogueLua.SetVariable("Archipelago_SQ02_State1_Item", true);
				return;
		}
	}
	
	// side effect: Adds the location to LocationFinder.UnscoutedLocations if necessary
	private static ItemData chooseItemData(long id) {
		if (id < 0) {
			return ItemGiver.APItemData;
		}
		ItemData itemData;
		if (SessionTools.LocationScouts != null) {
			Archipelago.MultiClient.Net.Models.ScoutedItemInfo itemInfo = SessionTools.LocationScouts[id];
			if (itemInfo.Player.Equals(SessionTools.ActivePlayer)) {
				itemData = ItemGiver.GetItemData(itemInfo.ItemId);
			} else {
				itemData = UnityEngine.Object.Instantiate(ItemGiver.APItemData).Cast<ItemData>();
				itemData._NameID = itemInfo.ItemDisplayName;
				itemData._DescriptionID = "An item from another world. It belongs to " +
					itemInfo.Player.Name + " in " + itemInfo.ItemGame + ".";
			}
		} else if (ItemGiver.ItemsBeforeLocations.ContainsKey(id)) { // these will always be local
			itemData = ItemGiver.GetItemData(ItemGiver.ItemsBeforeLocations[id]);
		} else {
			itemData = ItemGiver.APItemData;
			LocationFinder.UnscoutedLocations.Add(id);
		}
		return itemData;
	}
	
	// BoydCS notes
	// has properties _SerializationDataStateNCompleted where N is 0 through 5
	// Corresponding variable names are Quest.SQ02_StateN_Completed
	// State 0 Completed is true after the initial conversation with Boyd
	// State 1 Completed is true after taking the wrench from the arena chest
	// State 2 Completed is true after returning the wrench (and getting the blueprints)
	// State 3 Completed is true after building the tracking center
	// State 4 Completed is true after getting the SAFE kit
	// For the last stage, we're using Z00SQ02CS instead (same property name system)
	// State 5 Completed is true after upgrading the SAFE and getting the executioner
	
	public static void HandleCinematic(BoydCS cinematic) {
		if (DialogueLua.GetVariable(cinematic._SerializationDataState0Completed.VariableName, false) &&
			DialogueLua.GetVariable(cinematic._SerializationDataState1Completed.VariableName, false)) {
			long id = -1;
			string completed2 = cinematic._SerializationDataState2Completed.VariableName;
			if (!DialogueLua.GetVariable(completed2, false)) {
				DialogueLua.SetVariable("Archipelago_SQ02_State2_Location", true);
				id = LocationFinder.IdFromSerializationData(completed2);
				cinematic._BlueprintBoyd = chooseItemData(id);
			} else if (DialogueLua.GetVariable(cinematic._SerializationDataState3Completed.VariableName, false)) {
				string completed4 = cinematic._SerializationDataState4Completed.VariableName;
				if (!DialogueLua.GetVariable(completed4, false)) {
					id = LocationFinder.IdFromSerializationData(completed4);
					cinematic._ItemDataSAFEUpgrade = chooseItemData(id);
				}
			}				
			if (id > 0) {
				SessionTools.SendLocation(id);
			}
		}
		return;
	}
	
	public static void HandleCinematic(Z00SQ02CS cinematic) {
		string completed5 = cinematic._SerializationDataState5Completed.VariableName;
		if (!DialogueLua.GetVariable(completed5, false)) {
			long id = LocationFinder.IdFromSerializationData(completed5);
			cinematic._ItemDataChip = chooseItemData(id);
			if (id > 0) {
				SessionTools.SendLocation(id);
			}
		}
		return;
	}
}