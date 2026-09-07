using Il2CppLDS.MindBreaker.Cinematics;
using Il2CppPixelCrushers.DialogueSystem;

namespace BiomorphRandomizer;

public static class Quests {
	
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
		if (DialogueLua.GetVariable(cinematic._SerializationDataState1Completed.VariableName, false)) {
			long id = -1;
			string completed2 = cinematic._SerializationDataState2Completed.VariableName;
			if (!DialogueLua.GetVariable(completed2, false)) {
				cinematic._BlueprintBoyd = ItemGiver.APItemData;
				id = LocationFinder.IdFromSerializationData(completed2);
			} else if (DialogueLua.GetVariable(cinematic._SerializationDataState3Completed.VariableName, false)) {
				string completed4 = cinematic._SerializationDataState4Completed.VariableName;
				if (!DialogueLua.GetVariable(completed4, false)) {
					cinematic._ItemDataSAFEUpgrade = ItemGiver.APItemData;
					id = LocationFinder.IdFromSerializationData(completed4);
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
			cinematic._ItemDataChip = ItemGiver.APItemData;
			long id = LocationFinder.IdFromSerializationData(completed5);
			if (id > 0) {
				SessionTools.SendLocation(id);
			}
		}
		return;
	}
}