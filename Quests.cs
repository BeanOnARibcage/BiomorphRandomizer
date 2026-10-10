using Il2CppLDS.MindBreaker.Cinematics;
using Il2CppPixelCrushers.DialogueSystem;
using Il2CppLDS.MindBreaker.Data;

namespace BiomorphRandomizer;

public static class Quests {
	
	// Quest variables I'm messing with:
	// Quest.SQ02_State1_Completed
	// Quest.SQ20_State1_Completed
	// Global.SAFEUpgradeUnlocked
	// Quest.SQ35_State1_Completed
	public static void SetQuestVariables(string map) {
		switch (map) {
			case "Z00_02":
				// Boyd
				DialogueLua.SetVariable("Quest.SQ02_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ02_State1_Item", false));
				// Marle Kertar
				DialogueLua.SetVariable("Quest.SQ20_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ20_State1_Item", false));
				goto case "Z03_06";
			case "Z04_01":
				// Boyd
				DialogueLua.SetVariable("Quest.SQ02_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ02_State1_Location", false));
				return;
			case "Z03_06":
				// Boyd
				DialogueLua.SetVariable("Global.SAFEUpgradeUnlocked",
					DialogueLua.GetVariable("Quest.SQ02_State4_Completed", false) &&
					DialogueLua.GetVariable("Global.SAFEKits", 0) > 0);
				return;
			case "Z04_06":
				// Marle Kertar
				DialogueLua.SetVariable("Quest.SQ20_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ20_State1_Location", false));
				return;
			case "Z04_10":
				// Meed
				DialogueLua.SetVariable("Quest.SQ35_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ35_State1_Location", false));
				return;
			case "Z04_14":
				// Meed
				DialogueLua.SetVariable("Quest.SQ35_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ35_State1_Item", false));
				return;
		}
	}
	
	public static void CheckForQuestItem(long id) {
		switch (id) {
			case 820:
				DialogueLua.SetVariable("Archipelago_SQ02_State1_Item", true);
				return;
			case 316:
				DialogueLua.SetVariable("Archipelago_SQ20_State1_Item", true);
				return;
			case 1:
				DialogueLua.SetVariable("Archipelago_SQ35_State1_Item", true);
				return;
		}
	}
	
	public static void CheckForQuestLocation(long id) {
		switch (id) {
			case 401:
				DialogueLua.SetVariable("Archipelago_SQ02_State1_Location", true);
				return;
			case 406:
				DialogueLua.SetVariable("Archipelago_SQ20_State1_Location", true);
				return;
			case 410:
				DialogueLua.SetVariable("Archipelago_SQ35_State1_Location", true);
				return;
		}
	}
	
	// side effect: Adds the location to LocationFinder.UnscoutedLocations if necessary
	private static ItemData chooseItemData(long id) {
		if (id < 0) {
			return ItemGiver.APItemData;
		}
		ItemData itemData = LocationFinder.ItemBeingFound(id, true);
		return itemData;
	}
	
	// BaseDialogueCS notes:
	// use _ItemToGiveOnEnd and _SerializationDataItemToGiveOnEnd
	// Conversations:
	// Will in Mezzo Skyway
	// Will in Blightmoor
	
	public static void HandleCinematic(BaseDialogueCS cinematic) {
		string locationData = cinematic._SerializationDataItemToGiveOnEnd.VariableName;
		long id = LocationFinder.IdFromSerializationData(locationData);
		cinematic._ItemToGiveOnEnd = chooseItemData(id);
		SessionTools.SendLocation(id);
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
				// DialogueLua.SetVariable("Archipelago_SQ02_State2_Location", true);
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
	
	// Z04SQ35CS notes
	// Quest.SQ35_...
	// State 1 Completed is (presumably) when you get the letter-tapper
	// State 2 Completed is when you show it to Meed in the Skyway
	// State 3 Completed is when you get the blueprint in Blightmoor
	// swap to Z00SQ10CS
	// State 4 Completed is when you build the lab
	// State 5 Completed is when you talk to Meed in the lab
	
	public static void HandleCinematic(Z04SQ35CS cinematic) {
		if (DialogueLua.GetVariable("Quest.SQ35_State2_Completed", false) &&
			!DialogueLua.GetVariable("Quest.SQ35_State3_Completed", false)) {
			long id = LocationFinder.IdFromSerializationData("Quest.SQ35_State3_Completed");
			cinematic._BlueprintLab = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
	}
	
	// Z00SQ20CS notes
	// Quest.SQ20_...
	// State 1 Completed is (presumably) when you get the blueprint
	// State 2 Completed is when you build the restorer's shop
	// State 3 Completed is when Marle thanks you and gives you the chip imprinter blueprint
	// State 4 Completed is when you build the chip imprinter
	// swap to Z00RobotCS
	// State 5 Completed is when you talk to the chip imprinter
	
	public static void HandleCinematic(Z00SQ20CS cinematic) {
		if (DialogueLua.GetVariable("Quest.SQ20_State2_Completed", false) &&
			!DialogueLua.GetVariable("Quest.SQ20_State3_Completed", false)) {
			long id = LocationFinder.IdFromSerializationData("Quest.SQ20_State3_Completed");
			cinematic._BlueprintChip = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
	}
}