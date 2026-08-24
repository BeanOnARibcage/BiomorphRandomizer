using HarmonyLib;
using System.Threading.Tasks;
using Il2CppLDS.Sardonyx.Actions;
using MelonLoader;
using Il2CppLDS.Sardonyx.Actors;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.Framework.Core;
using Il2CppLDS.MindBreaker.UI;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	//ActionPickItem.StartExecution is called once (or twice?) per pickup, so that's a good one to patch
	[HarmonyPatch(typeof(ActionPickItem), nameof(ActionPickItem.StartExecution))]
	[HarmonyPrefix]
	static void PickItem(ActionPickItem __instance) {
		InteractionPickItem interaction = __instance.Interaction;
		if (interaction.name == "Prefab_Interaction_Money(Clone)") {
			return;
		}
		long id = LocationFinder.IntroIdFromInteractionName(interaction.name);
		if (id < 0) {
			return;
		}
		SessionTools.SendLocation(id);
		interaction.ItemQuantity = 0;
		//Melon<Randomizer>.Logger.Msg(__instance.Interaction.name);
		return;
	}
	
	[HarmonyPatch(typeof(SaveSlotUI), nameof(SaveSlotUI.UISaveSlotClick))]
	[HarmonyPrefix]
	static void Connect(SaveSlotUI __instance) {
		if (!SessionTools.CheckConnection()) {
			SessionTools.Connect();
		}
		if (SessionTools.CheckConnection() && __instance._GameData != null) {
			LocationsAlreadyFound locs = new LocationsAlreadyFound(__instance._GameData);
			SessionTools.SendMultipleLocations(locs.LocationIds);	
		}	
	}
	
	/* Hopefully I can do this in the SaveSlotUI.UISaveSlotClick patch instead
	[HarmonyPatch(typeof(GameManager.__c__DisplayClass113_0), 
		nameof(GameManager.__c__DisplayClass113_0._StateTitleScreen_b__0))]
	[HarmonyPrefix]
	static void ReadSaveFile0(GameData gameData) {
		Melon<Randomizer>.Logger.Msg("Reading save file 0");
		LocationsAlreadyFound locs = new LocationsAlreadyFound(gameData);
		SessionTools.SendMultipleLocations(locs.LocationIds); // I need to find a later time to do this
	}
	
	[HarmonyPatch(typeof(GameManager.__c__DisplayClass113_0), 
		nameof(GameManager.__c__DisplayClass113_0._StateTitleScreen_b__1))]
	[HarmonyPrefix]
	static void ReadSaveFile1(GameData gameData) {
		Melon<Randomizer>.Logger.Msg("Reading save file 1");
		LocationsAlreadyFound locs = new LocationsAlreadyFound(gameData);
		SessionTools.SendMultipleLocations(locs.LocationIds);
	}
	
	[HarmonyPatch(typeof(GameManager.__c__DisplayClass113_0), 
		nameof(GameManager.__c__DisplayClass113_0._StateTitleScreen_b__2))]
	[HarmonyPrefix]
	static void ReadSaveFile2(GameData gameData) {
		Melon<Randomizer>.Logger.Msg("Reading save file 2");
		LocationsAlreadyFound locs = new LocationsAlreadyFound(gameData);
		SessionTools.SendMultipleLocations(locs.LocationIds);
	} */
}