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
using Il2CppLDS.MindBreaker.GameplayObjects;
using Il2CppLDS.MindBreaker.Cinematics;
using Il2CppLDS.MindBreaker.Actions;
using Il2CppInterop.Runtime;
using Il2CppLDS.Framework.Cinematics;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	//ActionPickItem.StartExecution is called once (or twice?) per pickup, so that's a good one to patch
	[HarmonyPatch(typeof(ActionPickItem), nameof(ActionPickItem.StartExecution))]
	[HarmonyPrefix]
	static bool PickItem(ActionPickItem __instance) {
		Melon<Randomizer>.Logger.Msg("ActionPickItem prefix entered");
		InteractionPickItem interaction = __instance.Interaction;
		if (!interaction._ShowNotification) { // Should always be money
			return true;
		}
		long id = LocationFinder.IdFromSerializationData(interaction.SerializationData.VariableName);
		if (id < 0) {
			return true;
		}
		if (id == 301) {
			if (SessionTools.SlotData == null) {
				return false;
			}
			long itemId = (long)SessionTools.SlotData["starting_weapon"];
			interaction._ItemData = ItemGiver.GetItemData(itemId);
			if (itemId == 418) {
				ItemGiver.BruisersReceived = true;
			}
			SessionTools.SendLocation(id);
			return true;
		}
		SessionTools.SendLocation(id);
		interaction.ItemQuantity = 0;
		interaction._ItemData = ItemGiver.APItemData;
		return true;
	}
	
	// [HarmonyPatch(typeof(SaveHandler))]
	// [HarmonyPatch(nameof(SaveHandler.Autosave))]
	// [HarmonyPatch(new Type[] {typeof(UnityEngine.Vector2),typeof(bool),typeof(string),typeof(bool),typeof(bool)})]
	// [HarmonyPrefix]
	[HarmonyPatch(typeof(Z03MeleeWeapon2CS), nameof(Z03MeleeWeapon2CS.PostSequenceInternal))]
	[HarmonyPrefix]
	static void ManageStartingWeapon() {
		// if (mapName != "Z03_08") {
		// 	return;
		// }
		if ((long)SessionTools.SlotData["starting_weapon"] == 418) {
			return;
		} else {
			long startingWeaponId = (long)SessionTools.SlotData["starting_weapon"];
			WeaponData startingWeapon = ItemGiver.GetItemData(startingWeaponId).Cast<WeaponData>();
			ItemData bruisers = ItemGiver.GetItemData(418);
			InventoryHandler.UpdateItem(bruisers, 0);
			
			InventoryHandler.EquippedChip0 = startingWeapon;
			InventoryHandler.BaseHeroAttacks[0] = startingWeapon;
			InventoryHandler.Attacks[0] = startingWeapon;
			
			ActionWeapon actionWeapon = GameManager.Hero.GetComponentInChildren<ActionWeapon>();
			actionWeapon.OnWeaponChanged(0, startingWeapon);
			
			HUDScreen hud = UnityEngine.Object.FindAnyObjectByType<HUDScreen>();
			HUDAttackUI socket = hud._ChipSockets[0];
			socket.OnWeaponChanged(0, startingWeapon);
		}
		return;
	}
	
	[HarmonyPatch(typeof(Z03Beacon1CS), nameof(Z03Beacon1CS.PreSequenceInternal))]
	[HarmonyPrefix]
	static void RemoveBruisers() {
		if (!ItemGiver.BruisersReceived) {
			ItemData bruisers = ItemGiver.GetItemData(418);
			InventoryHandler.UpdateItem(bruisers, 0);
		}
	}
	
	[HarmonyPatch(typeof(ActionInteractionShop), nameof(ActionInteractionShop.StartExecution))]
	[HarmonyPrefix]
	static void Shop(ActionInteractionShop __instance) {
		InteractionShop interaction = __instance.Interaction;
		ShopItemData shopItemData;
		for (int i = 0; i < interaction.Items.Count; i++) {
			shopItemData = interaction.Items[i];
			long id = LocationFinder.IdFromSerializationData(shopItemData.VariableName);
			if (id < 0) {
				return;
			}
			// SessionTools.SendLocation(id); need to check whether they actually buy the item
			shopItemData._ItemData = ItemGiver.APItemData;
			// I don't think there's a way to make it give 0 of the item like with InteractionPickItem
		}
	}
			
	
	[HarmonyPatch(typeof(SaveSlotUI), nameof(SaveSlotUI.UISaveSlotClick))]
	[HarmonyPrefix]
	static void Connect(SaveSlotUI __instance) {
		SessionTools.ReceivingItemsOkay = false;
		if (!SessionTools.CheckConnection()) {
			SessionTools.Connect();
		}
		SessionTools.ItemsProcessed = DialogueLua.GetVariable("Archipelago_Items", 0);
		Melon<Randomizer>.Logger.Msg("Items already processed: " + SessionTools.ItemsProcessed.ToString());
		if (SessionTools.CheckConnection()) {
			SessionTools.ReceivingItemsOkay = true;
		}
	}
	
	[HarmonyPatch(typeof(ActionCinematic), nameof(ActionCinematic.StartExecution))]
	[HarmonyPrefix]
	static void DetermineWhichDialogue(ActionCinematic __instance) {
		CinematicSequence cinematic = __instance.Interaction.CinematicSequence;
		Il2CppSystem.Type cinematicType = Il2CppType.TypeFromPointer(cinematic.ObjectClass);
		if (cinematicType == Il2CppType.Of<BoydCS>()) {
			Quests.HandleCinematic(cinematic.Cast<BoydCS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<Z00SQ02CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z00SQ02CS>());
			return;
		}
		return;
	}	
	
	[HarmonyPatch(typeof(SaveHandler), nameof(SaveHandler.SaveGame))]
	[HarmonyPrefix]
	static void RecordItemsProcessed() {
		Melon<Randomizer>.Logger.Msg("SaveGame called");
		DialogueLua.SetVariable("Archipelago_Items", SessionTools.ItemsProcessed);
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
		LocationFinder.FillLocationDictionary();
		// if this takes too long, it can be made async
	}
	
	// It should (hopefully) be fine to use ToggleZoneActiveAsync on individual rooms (true to load, false to unload)
	/*
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
	*/
}