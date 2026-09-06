using Il2CppLDS.Framework.Inventory;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.MindBreaker.Data;
using UnityEngine;
using MelonLoader;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Reflection;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Il2CppLDS.Sardonyx.Actions;
using System.Collections.Generic;
using Il2CppLDS.Framework.Core;

namespace BiomorphRandomizer;

public static class ItemGiver {
	public static Scene APScene;
	public static GameObject APPickItemGO = null;
	public static InteractionPickItem APPickItem = null;
	public static ItemData APItemData = null;
	public static GameObject APItemGO = null;
	public static InteractionPickItem APItemInteraction = null;
	// use APPickItem for receiving items (subbing in the corresponding ItemData)
	// use APItemData for sending locations (putting it into the game's existing InteractionPickItem)
	
	public static bool CanGetItem() {
		return GameManager.CanUseInteractions && APPickItem != null;
	}
	
	public static void GiveChip(Chips chip) {
		WeaponData chipData = InventoryHandler.ItemDatabase.Chips[(int)chip];
		InventoryHandler.UpdateItem((ItemData)chipData, 1);
	}
	
	public static void IntroGiveItemFromId(int id) {
		if (id < 1 || id > 5) {
			return;
		}
		ItemData item = null;
		if (id == 1) {
			item = InventoryHandler.ItemDatabase.RawMaterials;
		}
		if (id == 2) {
			item = InventoryHandler.ItemDatabase.Laurentium;
		}
		if (id == 3) {
			item = InventoryHandler.ItemDatabase.VitalModules;
		}
		if (id == 4) {
			item = InventoryHandler.ItemDatabase.LogicBlocks;
		}
		if (id == 5) {
			item = (ItemData)InventoryHandler.ItemDatabase.MementoHorseshoeMagnet;
		}
		if (item != null) {
			InventoryHandler.UpdateItem(item, 1);
		}
		return;
	}
	
	public static void GiveItemFromId(long id) {
		// Clone the saved interaction because they're not reusable
		InteractionPickItem interaction = UnityEngine.Object.Instantiate(APPickItem, APScene).Cast<InteractionPickItem>();
		interaction._ItemData = GetItemData(id);
		interaction.ExecutePickItem();
	}
	
	public static int FindItemCount(string variables) {
		int index = variables.IndexOf("ArchipelagoItems");
		if (index == -1)
			return 0;
		else {
			if (int.TryParse(variables.Substring(index + 17, 3), out int number))
				return number;
			else
				return 0;
		}
	}
	
	// Running the coroutines manually because Unity's system doesn't even
	// give you a way to check if the coroutine is done, let alone get
	// information from it (such as a reference to the scene that was loaded)
	private static Il2CppSystem.Collections.IEnumerator coroutine1 = null;
	private static Il2CppSystem.Collections.IEnumerator coroutine2 = null;
	private static Il2CppSystem.Collections.IEnumerator coroutine3 = null;
	private static AsyncOperationHandle<SceneInstance> sceneHandle = null;
	private static bool loading = false;
	private static bool waiting = false;
	private static bool inProgress = false;
	
	public static void StartGetInteractions() {
		if (inProgress) {
			return;
		}
		coroutine1 = SceneHandler.ToggleZoneActiveAsync("Z04_10", true);
		inProgress = true;
		loading = true;
		waiting = false;
	}
	
	public static void GetInteractions() {
		if (coroutine1 != null) {
			if (sceneHandle != null && sceneHandle.IsDone) {
				waiting = false;
				try {
					if (loading) {
						Melon<Randomizer>.Logger.Msg("Point 1");
						//CreateSceneParameters csp = new CreateSceneParameters();
						Melon<Randomizer>.Logger.Msg("Point 1.1");
						//APScene = SceneManager.CreateScene("Archipelago", csp); // may have been stripped
						APScene = SceneManager.GetSceneByName("Master");
						Melon<Randomizer>.Logger.Msg("Point 1.2");
						GameObject laptopInteraction = GameObject.Find("/Root/GameplayAssets/Loot/Prefab_Interaction_LogBook");
						if (laptopInteraction == null) {
							Melon<Randomizer>.Logger.Msg("Unable to find the interaction GameObject");
							sceneHandle = null;
							return;
						}
						// UnityEngine.Object.Instantiate makes a clone
						APPickItemGO = UnityEngine.Object.Instantiate(laptopInteraction, APScene).Cast<GameObject>();
						APPickItem = APPickItemGO.GetComponent<InteractionPickItem>();
						if (APPickItem == null) {
							Melon<Randomizer>.Logger.Msg("The found GameObject does not have an InteractionPickItem");
							sceneHandle = null;
							return;
						}
						// Clone the entire GameObject again so that APItemData will have a reference in the il2cpp domain
						// Otherwise it gets garbage collected there and then freezes the game when accessed here
						APItemGO = UnityEngine.Object.Instantiate(APPickItemGO, APScene).Cast<GameObject>();
						APItemInteraction = APItemGO.GetComponent<InteractionPickItem>();
						APItemInteraction._ItemData = UnityEngine.Object.Instantiate(GetItemData(1)).Cast<ItemData>();
						APItemData = APItemInteraction._ItemData;
						if (APItemData == null) {
							Melon<Randomizer>.Logger.Msg("The copy of laptopData is null");
						} else {
							Melon<Randomizer>.Logger.Msg("The copy of laptopData is not null");
						}
						APPickItem._SerializationData = null;
						APPickItem._SerializationDataQuest = null;
						APPickItem._SerializationDataState = null;
						APItemData._NameID = "Randomized Item";
						APItemData._DescriptionID = "A randomized item from Archipelago.";
						sceneHandle = null;
					}
					else {
						Melon<Randomizer>.Logger.Msg("Point 2");
						sceneHandle = null;
					}
				} 
				finally {
					sceneHandle = null;
				}
			}
			else if (waiting) {
				Melon<Randomizer>.Logger.Msg("Point 3");
				return;
			}
			else if (coroutine3 != null && coroutine3.MoveNext()) {
				Melon<Randomizer>.Logger.Msg("Point 4");
				if (Il2CppType.TypeFromPointer(coroutine3.Current.ObjectClass)
					== Il2CppType.Of<AsyncOperationHandle<SceneInstance>>()) {
					Melon<Randomizer>.Logger.Msg("Point 5");
					sceneHandle = coroutine3.Current.Cast<AsyncOperationHandle<SceneInstance>>();
					waiting = true;
				}
			}
			else if (coroutine2 != null && coroutine2.MoveNext()) {
				Melon<Randomizer>.Logger.Msg("Point 6");
				coroutine3 = coroutine2.Current.Cast<Il2CppSystem.Collections.IEnumerator>();
			}
			else if (coroutine1.MoveNext()) {
				Melon<Randomizer>.Logger.Msg("Point 7");
				coroutine2 = coroutine1.Current.Cast<Il2CppSystem.Collections.IEnumerator>();
			}
			else if (coroutine2 != null && coroutine3 != null && sceneHandle == null) {
				if (loading) {
					Melon<Randomizer>.Logger.Msg("Point 8");
					coroutine1 = SceneHandler.ToggleZoneActiveAsync("Z04_10", false);
					coroutine2 = null;
					coroutine3 = null;
					loading = false;
				}
				else {
					Melon<Randomizer>.Logger.Msg("Point 9");
					coroutine1 = null;
					coroutine2 = null;
					coroutine3 = null;
					inProgress = false;
				}
			}
		}
	}
	
	public static ItemData GetItemData(long id) {
		int ones, hundreds;
		ItemDatabase items = InventoryHandler.ItemDatabase;
		hundreds = Math.DivRem((int)id, 100, out ones);
		switch (hundreds) {
			case 1: // Abilities
				return items.Abilities[ones];
			case 3: // Blueprints
				return items.Blueprints[ones].Cast<ItemData>();
			case 4: // Chips
				return items.Chips[ones];
			case 5: // Mementos
				return items.Mementos[ones];
			case 6: // Scargatos
				return items.Scargatos[ones];
			case 8: // Other key items
				return items.KeyItems[ones];
			case 0: // Other
				if (ones == 1)
					return items.Laptops.Cast<ItemData>();
				else if (ones == 2)
					return items.MementoSocket;
				else
					return null;
			default:
				return null;
		}
	}
}

public enum Chips {
	FerroxField = 0,
	CalamityBringer = 3,
	ScargatoWrath = 6,
	Executioner = 9,
	ShadowSting = 12,
	PainLegion = 15,
	Bruisers = 18,
	BarbDrizzle = 21,
	ForsakenEater = 24,
	PhoenixBlow = 27,
	LittleHelper = 30,
	FerroxSpecter = 33,
	FerroxRoar = 36,
	MechaCrash = 39,
	FerroxHurricane = 42
};