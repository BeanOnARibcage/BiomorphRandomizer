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
using Il2CppLDS.Framework.Actors;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	private static void log(string message) {
		Melon<Randomizer>.Logger.Msg(message);
	}
			
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
			interaction._ItemData = ItemGiver.GetItemData(itemId, true);
			if (itemId == 418) {
				ItemGiver.BruisersReceived = true;
			}
			SessionTools.SendLocation(id);
			return true;
		}
		SessionTools.SendLocation(id);
		interaction._ItemData = LocationFinder.ItemBeingFound(id, true);
		/*
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
		*/
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
			WeaponData startingWeapon;
			if (startingWeaponId / 100 == 4) { // starting weapon is a chip
				startingWeapon = ItemGiver.GetItemData(startingWeaponId, false).Cast<WeaponData>();
			} else if (startingWeaponId / 100 == 2) { // starting weapon is a biomorph
				startingWeapon = null;
			} else {
				Melon<Randomizer>.Logger.Msg("Invalid id for starting weapon: " +
					startingWeaponId.ToString());
				startingWeapon = null;
			}
			
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
	
	private static bool shop2Finished = false, shop3Finished = false;
	public static InteractionPickItem WaitingBoydInteraction = null;
	
	[HarmonyPatch(typeof(BoydCS), nameof(BoydCS.PreSequenceInternal))]
	[HarmonyPrefix]
	static void CacheBoydVariables() {
		shop2Finished = DialogueLua.GetVariable("NPCs.Boyd_Shop2_Finished", false);
		shop3Finished = DialogueLua.GetVariable("NPCs.Boyd_Shop3_Finished", false);
	}
	
	[HarmonyPatch(typeof(BoydCS), nameof(BoydCS.PostSequenceInternal))]
	[HarmonyPrefix]
	static bool TrackingCenterRewards() {
		Melon<Randomizer>.Logger.Msg("BoydCS prefix entered");
		if (DialogueLua.GetVariable("Quest.SQ02_State5_Completed", false)) {
			log("first if block entered");
			if (DialogueLua.GetVariable("ShopBuildingData_BoydShop_03", false) && !shop3Finished) {
				Melon<Randomizer>.Logger.Msg("tracking center 3 branch entered");
				return true; // temporary true, until the location (id 55) is in scope
				//return false;
			} else if (DialogueLua.GetVariable("ShopBuildingData_BoydShop_02", false) && !shop2Finished) {
				log("tracking center 2 branch entered");
				ItemData item = LocationFinder.ItemBeingFound(54, true);
				// DialogueLua.SetVariable("NPCs.Boyd_Shop2_Finished", true);
				// that variable gets set before PostSequenceInteral executes
				InteractionPickItem interaction = UnityEngine.Object.Instantiate(
					ItemGiver.APPickItem, ItemGiver.APScene).Cast<InteractionPickItem>();
				interaction._ItemData = item;
				WaitingBoydInteraction = interaction;
				SessionTools.SendLocation(54);
				return false;
			}
		}
		return true;
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
			shopItemData._ItemData = LocationFinder.ItemBeingFound(id, false);
			/*
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
			*/
			// I don't think there's a way to make it give 0 of the item like with InteractionPickItem
		}
	}
	
	[HarmonyPatch(typeof(ShopScreen), nameof(ShopScreen.OnShopPurchaseSuccess))]
	[HarmonyPrefix]
	static void SendShopLocation(ShopItemData shopItemData, ShopScreen __instance) {
		long id = LocationFinder.IdFromSerializationData(shopItemData.VariableName);
		if (id < 0) {
			return;
		}
		long itemId = -1;
		if (SessionTools.LocationScouts != null && SessionTools.LocationScouts.ContainsKey(id)) {
			itemId = SessionTools.LocationScouts[id].ItemId;
			Quests.CheckForQuestItem(itemId);
		} else if (ItemGiver.ItemsBeforeLocations.ContainsKey(id)) {
			itemId = ItemGiver.ItemsBeforeLocations[id];
			Quests.CheckForQuestItem(itemId);
		}
		SessionTools.SendLocation(id);
		LocationFinder.ItemBeingFound(id, true); // for the side effects
		if (ItemGiver.ProgressiveItems.ContainsKey(itemId)) {
			ShopItemData otherData;
			long otherId;
			for (int i = 0; i < __instance.Items.Count; i++) {
				otherData = __instance.Items[i];
				if (otherData != shopItemData) {
					otherId = LocationFinder.IdFromSerializationData(otherData.VariableName);
					if (otherId > 0) {
						otherData._ItemData = LocationFinder.ItemBeingFound(otherId, false);
					}
				}
			}
		}
	}
	
	// [HarmonyPatch(typeof(SaveSlotUI), nameof(SaveSlotUI.UISaveSlotClick))]
	// [HarmonyPrefix]
	// static void Connect(SaveSlotUI __instance) {
	// }
	
	[HarmonyPatch(typeof(PersistentDataManager), nameof(PersistentDataManager.ApplySaveData))]
	[HarmonyPostfix]
	static void ReadArchipelagoData() {
		log("PersistentDataManager.ApplySaveData postfix entered");
		SessionTools.ItemsProcessed = DialogueLua.GetVariable("Archipelago_Items", 0);
		Melon<Randomizer>.Logger.Msg("Items already processed: " + SessionTools.ItemsProcessed.ToString());
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
		if (DialogueLua.DoesVariableExist("Archipelago_Progressive_Items") &&
			DialogueLua.GetVariable("Archipelago_Progressive_Items").isTable) {
			ItemGiver.FillProgressiveItems(TableHandler.TableToDictInt(
				DialogueLua.GetVariable("Archipelago_Progressive_Items").asTable.luaTable));
		} else {
			ItemGiver.FillProgressiveItems(null);
		}
		Biomorphs.CreateFreeBiomorphs();
		Biomorphs.ShouldApplyLoadedBiomorphRewards = true;
		if (!SessionTools.CheckConnection()) {
			SessionTools.Connect();
		}
		SessionTools.FirstConnectionAttempt = true;
		if (SessionTools.CheckConnection()) {
			LocationFinder.CheckForLocations();
		}
	}
	
	[HarmonyPatch(typeof(ActionCinematic), nameof(ActionCinematic.StartExecution))]
	[HarmonyPrefix]
	static void DetermineWhichDialogue(ActionCinematic __instance) {
		CinematicSequence cinematic = __instance.Interaction.CinematicSequence;
		Il2CppSystem.Type cinematicType = Il2CppType.TypeFromPointer(cinematic.ObjectClass);
		if (cinematicType == Il2CppType.Of<BaseDialogueCS>()) {
			Quests.HandleCinematic(cinematic.Cast<BaseDialogueCS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<BoydCS>()) {
			Quests.HandleCinematic(cinematic.Cast<BoydCS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<Z00SQ02CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z00SQ02CS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<Z00SQ20CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z00SQ20CS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<Z04SQ35CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z04SQ35CS>());
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
		DialogueLua.SetVariable("Archipelago_Progressive_Items",
			TableHandler.DictToTable(ItemGiver.ProgressiveItems));
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
		BiomorphRewardHolder rewards = ItemGiver.APPickItemGO.AddComponent<BiomorphRewardHolder>();
		Biomorphs.SetAndFillBiomorphRewards(rewards);
		LocationFinder.FillLocationDictionary();
		// ItemGiver.FillProgressiveItems();
	}
	
	[HarmonyPatch(typeof(SceneHandler), nameof(SceneHandler.LoadMapsInternalAsync))]
	[HarmonyPrefix]
	static void SetVariables(string zone) {
		Quests.SetQuestVariables(zone);
		Biomorphs.ShuffleFreeBiomorphs(zone);
	}
	
	[HarmonyPatch(typeof(MindBreakArea), nameof(MindBreakArea.OnMindBreak))]
	[HarmonyPostfix]
	static void RecordBiomorph(MonsterData monsterData, SerializationData serializationDataMindBreak) {
		if (serializationDataMindBreak == null || monsterData == null) {
			return; // This method is also called when turning back into Harlo
		}
		Biomorphs.RecordBiomorph(monsterData._TextTable.GetFieldTextForLanguage(monsterData._NameID, 1),
			serializationDataMindBreak.VariableName);
	}
	
	private static bool PrefixForApply(BiomorphRewardData instance) {
		Melon<Randomizer>.Logger.Msg("PrefixForApply entered");
		long locationId;
		if (instance._RewardID.StartsWith("AP_")) { //code that I'm the one who called the method
			instance._RewardID = instance._RewardID.Substring(3);
			return true;
		} else if (instance.ItemData != null && long.TryParse(instance.ItemData._NameID, out locationId)) {
			ItemData item = LocationFinder.ItemBeingFound(locationId, true);
			instance._ItemData = item;
			SessionTools.SendLocation(locationId);
			LocationFinder.RecordLocationSerializationData(locationId);
			// the notification for the item is already displayed, so we give the item silently
			InventoryHandler.UpdateItem(item, 1);
			return false;
		} else {
			return true; // Unrandomized reward
		}
	}
	
	[HarmonyPatch(typeof(BiomorphRewardData), nameof(BiomorphRewardData.Apply))]
	[HarmonyPrefix]
	static bool Arsenal(BiomorphRewardData __instance) {
		return PrefixForApply(__instance);
	}
	
	[HarmonyPatch(typeof(BiomorphRewardDataArsenal), nameof(BiomorphRewardDataArsenal.Apply))]
	[HarmonyPrefix]
	static bool Arsenal(BiomorphRewardDataArsenal __instance) {
		return PrefixForApply(__instance);
	}
	
	[HarmonyPatch(typeof(BiomorphRewardDataAttack), nameof(BiomorphRewardDataAttack.Apply))]
	[HarmonyPrefix]
	static bool Attack(BiomorphRewardDataAttack __instance) {
		return PrefixForApply(__instance);
	}
	
	[HarmonyPatch(typeof(BiomorphRewardDataCharge), nameof(BiomorphRewardDataCharge.Apply))]
	[HarmonyPrefix]
	static bool Charge(BiomorphRewardDataCharge __instance) {
		return PrefixForApply(__instance);
	}
	
	[HarmonyPatch(typeof(BiomorphRewardDataChip), nameof(BiomorphRewardDataChip.Apply))]
	[HarmonyPrefix]
	static bool Chip(BiomorphRewardDataChip __instance) {
		return PrefixForApply(__instance);
	}
	
	[HarmonyPatch(typeof(BiomorphRewardDataDefense), nameof(BiomorphRewardDataDefense.Apply))]
	[HarmonyPrefix]
	static bool Defense(BiomorphRewardDataDefense __instance) {
		return PrefixForApply(__instance);
	}
	/*
	private static string biomorphsStem = null;
	
	[HarmonyPatch(typeof(ProgressHandler), nameof(ProgressHandler.OnMindBreak))]
	[HarmonyPrefix]
	static void OuterPrefix(MonsterData monsterData, SerializationData serializationData) {
		if (serializationData == null) {
			return;
		}
		if (monsterData._NameID == "") {
			return;
		}
		string monsterName = monsterData._TextTable.GetFieldTextForLanguage(monsterData._NameID, 1);
		if (Biomorphs.IsBiomorphable(monsterName)) {
			Biomorphs.TempRemoveFreeBiomorph(monsterName);
			biomorphsStem = serializationData.VariableName.Remove(serializationData.VariableName.Length - 2);
			Biomorphs.TempRemoveBiomorphs(biomorphsStem);
		}
	}
	
	[HarmonyPatch(typeof(MonsterData), nameof(MonsterData.EvaluateBiomorphReward))]
	[HarmonyPrefix]
	static void InnerPrefix(MonsterData __instance) {
		if (__instance._NameID == "") {
			return;
		}
		string monsterName = __instance._TextTable.GetFieldTextForLanguage(__instance._NameID, 1);
		if (Biomorphs.IsBiomorphable(monsterName)) {
			Biomorphs.TempRemoveFreeBiomorph(monsterName);
			
			if (biomorphsStem != null) {
				Biomorphs.RestoreBiomorphs(biomorphsStem);
			}
		}
	}
	
	[HarmonyPatch(typeof(MonsterData), nameof(MonsterData.EvaluateBiomorphReward))]
	[HarmonyPostfix]
	static void InnerPostfix(MonsterData __instance) {
		if (__instance._NameID == "") {
			return;
		}
		string monsterName = __instance._TextTable.GetFieldTextForLanguage(__instance._NameID, 1);
		Biomorphs.RestoreFreeBiomorph(monsterName);
		
		if (biomorphsStem != null) {
			Biomorphs.TempRemoveBiomorphs(biomorphsStem);
		}
	}
	*/
	[HarmonyPatch(typeof(ProgressHandler), nameof(ProgressHandler.OnMindBreak))]
	[HarmonyPostfix]
	static void OuterPostfix(MonsterData monsterData, SerializationData serializationData) {
		if (serializationData == null || monsterData == null || monsterData._NameID == ""
			|| monsterData._TextTable == null) {
			return;
		}
		string monsterName = monsterData._TextTable.GetFieldTextForLanguage(monsterData._NameID, 1);
		if (Biomorphs.IsBiomorphable(monsterName)) {
			// Biomorphs.RestoreBiomorphs(biomorphsStem);
			// biomorphsStem = null;
			// Biomorphs.RestoreFreeBiomorph(monsterName);
			if (!Biomorphs.IsMonsterUnlocked(monsterName) &&
				ProgressHandler.BiomorphRewardArsenal.Contains(monsterData)) {
				ProgressHandler.BiomorphRewardArsenal.Remove(monsterData);
			}
		}
	}
	
	[HarmonyPatch(typeof(ProgressHandler), nameof(ProgressHandler.OnLoad))]
	[HarmonyPostfix]
	static void GrantBiomorphRewards() {
		log("ProgressHandler.OnLoad postfix entered");
		// switch everything past this to Update() when the above bool is false
	}
	
	/*
	// Methods to log the scene-loading functions
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

// [HarmonyPatch]
public class Loggers {
	// [HarmonyPatch(typeof(DialogueLua), nameof(DialogueLua.SetVariable))]
	// [HarmonyPostfix]
	// static void LogSetVariable(string variable) {
	// 	Melon<Randomizer>.Logger.Msg("Variable " + variable + " is " +
	// 		DialogueLua.GetVariable(variable).AsString);
	// } //SetVariable isn't always used when a variable is set
	
	[HarmonyPatch(typeof(BiomorphRewardDataArsenal), nameof(BiomorphRewardDataArsenal.Apply))]
	[HarmonyPostfix]
	static void LogApply() {
		Melon<Randomizer>.Logger.Msg("Apply was called");
	}
}