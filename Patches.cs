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
using Il2CppLDS.MindBreaker.Data;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	//ActionPickItem.StartExecution is called once (or twice?) per pickup, so that's a good one to patch
	[HarmonyPatch(typeof(ActionPickItem), nameof(ActionPickItem.Execute))]
	[HarmonyPrefix]
	static void PickItem(ActionPickItem __instance) {
		Melon<Randomizer>.Logger.Msg("ActionPickItem prefix entered");
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
		interaction._ItemData = ItemGiver.APItemData;
		Melon<Randomizer>.Logger.Msg(__instance.Interaction.name);
		Melon<Randomizer>.Logger.Msg(__instance.Interaction != null);
		Melon<Randomizer>.Logger.Msg(interaction.name);
		Melon<Randomizer>.Logger.Msg(ItemGiver.APItemData != null);
		Melon<Randomizer>.Logger.Msg(interaction._ItemData != null);
		Melon<Randomizer>.Logger.Msg(interaction._ItemData.name);
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
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.CoroutineToggleZone))]
	[HarmonyPrefix]
	static void LogToggleZone(string zone, bool enable) {
		Melon<Randomizer>.Logger.Msg("CoroutineToggleZone zone: " + zone + " enable: " + enable.ToString());
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.LoadMapsInternalAsync))]
	[HarmonyPrefix]
	static void LogLMIA(string zone) {
		Melon<Randomizer>.Logger.Msg("LoadMapsInternalAsync zone: " + zone);
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.ReloadZone))]
	[HarmonyPrefix]
	static void LogReload(string zone) {
		Melon<Randomizer>.Logger.Msg("ReloadZone zone: " + zone);
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.ReloadZoneInternal))]
	[HarmonyPrefix]
	static void LogReloadInternal(string zone) {
		Melon<Randomizer>.Logger.Msg("ReloadZoneInternal zone: " + zone);
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.ToggleZoneActive))]
	[HarmonyPrefix]
	static void LogToggle(MapData map, bool enable) {
		Melon<Randomizer>.Logger.Msg("ToggleZoneActive map: " + map.ToString() + " enable: " + enable.ToString());
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.ToggleZoneActiveAsync))]
	[HarmonyPrefix]
	static void LogToggleA(string mapName, bool enable) {
		Melon<Randomizer>.Logger.Msg("ToggleZoneActiveAsync mapName: " + mapName +
			" enable: " + enable.ToString());
	}
	
	[HarmonyPatch(typeof(PanelSwitcherUI), nameof(PanelSwitcherUI.UISwitchPanel))]
	[HarmonyPrefix]
	static void GetInteractions(PanelSwitcherUI __instance) {
		if (__instance.name != "Start Game Button") {
			return;
		}
		if (ItemGiver.APItemData != null) {
			return;
		}
		ItemGiver.StartGetInteractions();
		ItemGiver.FillItemData();
	}
	
	// It should (hopefully) be fine to use ToggleZoneActiveAsync on individual rooms (true to load, false to unload)
}