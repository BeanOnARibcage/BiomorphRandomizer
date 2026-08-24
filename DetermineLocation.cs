using System.Collections.Generic;
using Il2CppLDS.MindBreaker.Core;
using Il2CppPixelCrushers.DialogueSystem;

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
}

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