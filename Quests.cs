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
	// Quest.SQ06_MetScavengerSon
	// Quest.SQ06 (integer)
	// Quest.SQ06_State1_Completed
	// Quest.MQ03_State1_Completed
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
			case "Z00_15":
				// Mémé
				DialogueLua.SetVariable("Game.ScargatosFound", ItemGiver.ScargatoCount);
				return;
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
			case "Z01_18":
				// Busko
				DialogueLua.SetVariable("Quest.SQ06_MetScavengerSon",
					DialogueLua.GetVariable("Archipelago_SQ06_Busko_Location", false));
				return;
			case "Z00_06":
				// Skipio
				DialogueLua.SetVariable("Quest.SQ06",
					DialogueLua.GetVariable("Archipelago_SQ06_Busko_Item", false) ? 1 : 0);
					// 1 if true, 0 if false
				DialogueLua.SetVariable("Quest.SQ06_State1_Completed",
					DialogueLua.GetVariable("Archipelago_SQ06_State1_Location", false));
				return;
			case "Z01_21":
				// Salm and Alma
				DialogueLua.SetVariable("Quest.MQ03_State1_Completed",
					DialogueLua.GetVariable("Archipelago_MQ03_State1_Location", false));
				return;
			case "Z01_08":
				// Salm and Alma
				DialogueLua.SetVariable("Quest.MQ03_State1_Completed",
					DialogueLua.GetVariable("Archipelago_MQ03_State1_Item", false));
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
			case 827:
				DialogueLua.SetVariable("Archipelago_SQ06_Busko_Item", true);
				return;
			case 828:
				DialogueLua.SetVariable("Archipelago_SQ06_State1_Item", true);
				return;
			case 829:
				DialogueLua.SetVariable("Archipelago_MQ03_State1_Item", true);
				return;
		}
	}
	
	public static void CheckForQuestLocation(long id) {
		switch (id) {
			case 53:
				DialogueLua.SetVariable("Archipelago_SQ35_State3_Location", true);
				return;
			case 401:
				DialogueLua.SetVariable("Archipelago_SQ02_State1_Location", true);
				return;
			case 406:
				DialogueLua.SetVariable("Archipelago_SQ20_State1_Location", true);
				return;
			case 410:
				DialogueLua.SetVariable("Archipelago_SQ35_State1_Location", true);
				return;
			case 107:
				DialogueLua.SetVariable("Archipelago_MQ03_State1_Location", true);
				return;
			case 121:
				DialogueLua.SetVariable("Archipelago_SQ06_Busko_Location", true);
				return;
			case 61:
				DialogueLua.SetVariable("Archipelago_SQ06_State1_Location", true);
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
	// All Will's conversations (presumably)
	
	public static void HandleCinematic(BaseDialogueCS cinematic) {
		if (cinematic._SerializationDataItemToGiveOnEnd != null &&
				cinematic._ItemToGiveOnEnd != null) {
			string locationData = cinematic._SerializationDataItemToGiveOnEnd.VariableName;
			long id = LocationFinder.IdFromSerializationData(locationData);
			cinematic._ItemToGiveOnEnd = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
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
	
	// Z01MQ03CS notes
	// Quest.MQ03_...
	// Quest.MQ03_SpokenOnce is when you first talk to Salm
	// State 1 Completed is when you take the ring
	// State 2 Completed is when you give it to Salm and he leaves for Blightmoor
	// 	(also starts SQ01)
	
	public static void HandleCinematic(Z01MQ03CS cinematic) {
		if (DialogueLua.GetVariable("Quest.MQ03_SpokenOnce", false) &&
			DialogueLua.GetVariable("Quest.MQ03_State1_Completed", false) &&
			!DialogueLua.GetVariable("Quest.MQ03_State2_Completed", false)) {
			long id = LocationFinder.IdFromSerializationData("Quest.MQ03_State2_Completed");
			cinematic._ItemDataAccessKey = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
	}
	
	// SalmKertarCS notes
	// Quest.SQ01_...
	// State 1 Completed is when you get the blueprint from Salm
	// State 2 Completed is when you build the shop
	// State 3 Completed is when you talk to Salm at the shop
	
	public static void HandleCinematic(SalmKertarCS cinematic) {
		if (!DialogueLua.GetVariable("Quest.SQ01_State1_Completed", false)) {
			long id = LocationFinder.IdFromSerializationData("Quest.SQ01_State1_Completed");
			cinematic._ItemDataBlueprintSalm = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
	}
	
	// Pay It Forward notes
	// Z01SQ06CS
	// Quest.SQ06 = 1 and MetScavengerSon are when you get the locking mechanism from Busko
	// (Busko checks MetScavengerSon and Skipio checks Quest.SQ06)
	// Z00SQ06CS
	// State 1 Completed is when you get the letter from Skipio
	
	public static void HandleCinematic(Z01SQ06CS cinematic) {
		if (!DialogueLua.GetVariable("Quest.SQ06_MetScavengerSon", false)) {
			long id = LocationFinder.IdFromSerializationData("Archipelago_SQ06_Busko_Location");
			cinematic._ItemHalfLocket = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
	}
	
	public static void HandleCinematic(Z00SQ06CS cinematic) {
		if (DialogueLua.GetVariable("Quest.SQ06", 0) > 0 &&
			!DialogueLua.GetVariable("Quest.SQ06_State1_Completed", false)) {
			long id = LocationFinder.IdFromSerializationData("Archipelago_SQ06_State1_Location");
			cinematic._ItemDataLetter = chooseItemData(id);
			SessionTools.SendLocation(id);
		}
	}
}