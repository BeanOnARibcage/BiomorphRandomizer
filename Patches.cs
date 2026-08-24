using HarmonyLib;
using System.Threading.Tasks;
using Il2CppLDS.Sardonyx.Actions;
using MelonLoader;
using Il2CppLDS.Sardonyx.Actors;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.Framework.Core;
using Il2CppLDS.MindBreaker.UI;
using UnityEngine;
using Il2CppPixelCrushers.DialogueSystem;

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
		SessionTools.ReceivingItemsOkay = false;
		if (!SessionTools.CheckConnection()) {
			SessionTools.Connect();
		}
		if (SessionTools.CheckConnection() && __instance._GameData != null) {
			LocationsAlreadyFound locs = new LocationsAlreadyFound(__instance._GameData);
			SessionTools.SendMultipleLocations(locs.LocationIds);
			//int itemCount = ItemGiver.FindItemCount(__instance._GameData.Variables);
			SessionTools.ItemsProcessed = DialogueLua.GetVariable("Archipelago_Items", 0);
		}
		if (SessionTools.CheckConnection()) {
			SessionTools.ReceivingItemsOkay = true;
		}
	}
	
	[HarmonyPatch(typeof(SaveHandler), nameof(SaveHandler.SaveGame))]
	[HarmonyPrefix]
	static void RecordItemsProcessed() {
		Melon<Randomizer>.Logger.Msg("SaveGame called");
		DialogueLua.SetVariable("Archipelago_Items", SessionTools.ItemsProcessed);
		/*string vars = SaveHandler.GameData.Variables;
		int index = vars.IndexOf("ArchipelagoItems");
		if (index == -1) {
			string entry = "ArchipelagoItems=" + SessionTools.ItemsProcessed.ToString("D3");
			vars = vars.TrimEnd().Insert(vars.Length - 2, entry);
		}
		else {
			vars = vars.Remove(index + 17, 3).Insert(index + 17, SessionTools.ItemsProcessed.ToString("D3"));
		}
		SaveHandler.GameData.Variables = vars;*/
	}
}