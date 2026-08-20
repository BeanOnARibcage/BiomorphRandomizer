

public static class LocationFinder {
	public static int IntroIdFromInteractionName(string interaction_name) {
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