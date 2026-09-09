using HarmonyLib;
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
using Il2CppLDS.Framework.AttributeModifiers;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	// ActionPickItem.StartExecution is called once per pickup, so that's a good one to patch
	[HarmonyPatch(typeof(ActionPickItem), nameof(ActionPickItem.StartExecution))]
	[HarmonyPrefix]
	static bool PickItem(ActionPickItem __instance) {
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
		if (SessionTools.LocationScouts != null) {
			Archipelago.MultiClient.Net.Models.ScoutedItemInfo itemInfo = SessionTools.LocationScouts[id];
			if (itemInfo.Player.Equals(SessionTools.ActivePlayer)) {
				interaction._ItemData = ItemGiver.GetItemData(itemInfo.ItemId);
			} else {
				ItemData remoteItem = UnityEngine.Object.Instantiate(ItemGiver.APItemData).Cast<ItemData>();
				remoteItem._NameID = itemInfo.ItemDisplayName;
				remoteItem._DescriptionID = "An item from another world. It belongs to " +
					itemInfo.Player.Name + " in " + itemInfo.ItemGame + ".";
				interaction._ItemData = remoteItem;
				interaction.ItemQuantity = 0;
			}
		} else if (ItemGiver.ItemsBeforeLocations.ContainsKey(id)) { // these will always be local
			interaction._ItemData = ItemGiver.GetItemData(ItemGiver.ItemsBeforeLocations[id]);
		} else {
			interaction._ItemData = ItemGiver.APItemData;
			interaction.ItemQuantity = 0;
			LocationFinder.UnscoutedLocations.Add(id);
		}
		return true;
	}
	
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
			WeaponData startingWeapon = ItemGiver.GetItemData(startingWeaponId, false).Cast<WeaponData>();
			ItemData bruisers = ItemGiver.GetItemData(418, false);
			//InventoryHandler.UpdateItem(bruisers, 0);
			
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
			ItemData bruisers = ItemGiver.GetItemData(418, false);
			InventoryHandler.UpdateItem(bruisers, 0);
		}
	}
	
	[HarmonyPatch(typeof(Z03CaptureCS), nameof(Z03CaptureCS.PostSequenceInternal))]
	[HarmonyPostfix]
	static void BreakTube() {
		GameObject tubeGO = GameObject.Find("/Root/States/Prefab_GO_Z03_FerroxVatCS");
		Attributes tubeAttributes = tubeGO.GetComponent<Attributes>();
		tubeAttributes.Health = 1;
		// May need Health=0 for certain starting weapons
	}
	
	[HarmonyPatch(typeof(Attributes), nameof(Attributes.OnEnable))]
	[HarmonyPostfix]
	static void BreakPath(Attributes __instance) {
		if (__instance.name == "Prefab_Destructible_Path_z03_01(Clone)" &&
		ProgressHandler.BeaconActivated > 0) {
			__instance.Health = 0;
			return;
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
			if (SessionTools.LocationScouts != null) {
				if (!SessionTools.LocationScouts.ContainsKey(id)) {
					return;
					// this happens for not-unlocked shop items (like Asrar has) that aren't
					// locations in the multiworld
				}
				Archipelago.MultiClient.Net.Models.ScoutedItemInfo itemInfo = SessionTools.LocationScouts[id];
				if (itemInfo.Player.Equals(SessionTools.ActivePlayer)) {
					shopItemData._ItemData = ItemGiver.GetItemData(itemInfo.ItemId, false);
				} else {
					ItemData remoteItem = UnityEngine.Object.Instantiate(ItemGiver.APItemData).Cast<ItemData>();
					remoteItem._NameID = itemInfo.ItemDisplayName;
					remoteItem._DescriptionID = "An item from another world. It belongs to " +
						itemInfo.Player.Name + " in " + itemInfo.ItemGame + ".";
					shopItemData._ItemData = remoteItem;
				}
			} else if (ItemGiver.ItemsBeforeLocations.ContainsKey(id)) { // these will always be local
				shopItemData._ItemData = ItemGiver.GetItemData(ItemGiver.ItemsBeforeLocations[id], false);
			} else {
				shopItemData._ItemData = ItemGiver.APItemData;
			}
			// I don't think there's a way to make it give 0 of the item like with InteractionPickItem
		}
	}
	
	[HarmonyPatch(typeof(ShopScreen), nameof(ShopScreen.OnShopPurchaseSuccess))]
	[HarmonyPrefix]
	static void SendShopLocation(ShopItemData shopItemData) {
		long id = LocationFinder.IdFromSerializationData(shopItemData.VariableName);
		if (id < 0) {
			return;
		}
		if (SessionTools.LocationScouts != null && SessionTools.LocationScouts.ContainsKey(id)) {
			Quests.CheckForQuestItem(SessionTools.LocationScouts[id].ItemId);
		} else if (ItemGiver.ItemsBeforeLocations.ContainsKey(id)) {
			Quests.CheckForQuestItem(ItemGiver.ItemsBeforeLocations[id]);
		}
		SessionTools.SendLocation(id);
		if (shopItemData._ItemData == ItemGiver.APItemData) {
			LocationFinder.UnscoutedLocations.Add(id);
		}
	}
	
	[HarmonyPatch(typeof(SaveSlotUI), nameof(SaveSlotUI.UISaveSlotClick))]
	[HarmonyPrefix]
	static void Connect(SaveSlotUI __instance) {
	}
	
	[HarmonyPatch(typeof(PersistentDataManager), nameof(PersistentDataManager.ApplySaveData))]
	[HarmonyPostfix]
	static void ReadArchipelagoData() {
		SessionTools.ItemsProcessed = DialogueLua.GetVariable("Archipelago_Items", 0);
		Melon<Randomizer>.Logger.Msg("Items already processed: " + SessionTools.ItemsProcessed.ToString());
		if (SessionTools.CheckConnection()) {
			LocationFinder.CheckForLocations();
		}
		if (DialogueLua.DoesVariableExist("Archipelago_UnscoutedLocations") &&
			DialogueLua.GetVariable("Archipelago_UnscoutedLocations").isTable) {
			LocationFinder.UnscoutedLocations = TableHandler.TableToList(
				DialogueLua.GetVariable("Archipelago_UnscoutedLocations").asTable.luaTable);
		}
		if (DialogueLua.DoesVariableExist("Archipelago_ItemsBeforeLocations") &&
			DialogueLua.GetVariable("Archipelago_ItemsBeforeLocations").isTable) {
			ItemGiver.ItemsBeforeLocations = TableHandler.TableToDict(
				DialogueLua.GetVariable("Archipelago_ItemsBeforeLocations").asTable.luaTable);
		}
		if (!SessionTools.CheckConnection()) {
			SessionTools.Connect();
		}
		SessionTools.FirstConnectionAttempt = true;
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
		if (LocationFinder.UnscoutedLocations.Count > 0) {
			DialogueLua.SetVariable("Archipelago_UnscoutedLocations",
				TableHandler.ListToTable(LocationFinder.UnscoutedLocations));
		} else {
			DialogueLua.SetVariable("Archipelago_UnscoutedLocations", new Il2CppLanguage.Lua.LuaNil());
		}
		if (ItemGiver.ItemsBeforeLocations.Count > 0) {
			DialogueLua.SetVariable("Archipelago_ItemsBeforeLocations",
				TableHandler.DictToTable(ItemGiver.ItemsBeforeLocations));
		} else {
			DialogueLua.SetVariable("Archipelago_ItemsBeforeLocations", new Il2CppLanguage.Lua.LuaNil());
		}
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
		ItemGiver.MakeAPInteraction();
		LocationFinder.FillLocationDictionary();
		// if this takes too long, it can be made async
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.ToggleZoneActive))]
	[HarmonyPrefix]
	static void SetQuestVariables(MapData map, bool enable) {
		if (enable) {
			string mapName = map.name;
			Quests.SetQuestVariables(mapName);
		}
	}
	/*
	[HarmonyPatch(typeof(DialogueLua), nameof(DialogueLua.SetVariable))]
	[HarmonyPostfix]
	static void LogSetVariable(string variable) {
		Melon<Randomizer>.Logger.Msg("Variable " + variable + " is " +
			DialogueLua.GetVariable(variable).AsString);
	}
	*/
}