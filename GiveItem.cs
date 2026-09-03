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
	// use APPickItem for receiving items (subbing in the corresponding ItemData)
	// use APItemData for sending locations (putting it into the game's existing InteractionPickItem)
	
	private static Dictionary<int, ItemData> itemDictionary;
	
	private static ItemData laptopData = null;
	
	public static bool CanGetItem() {
		return GameManager.Hero.GetComponent<ActionPickItem>().CanExecute();
		// I need to test whether this does what I want it to do
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
	
	public static void GiveItemFromId(int id) {
		APPickItem._ItemData = itemDictionary[id];
		APPickItem.ExecutePickItem();
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
	
	private static Il2CppSystem.Collections.IEnumerator coroutine1 = null;
	private static Il2CppSystem.Collections.IEnumerator coroutine2 = null;
	private static Il2CppSystem.Collections.IEnumerator coroutine3 = null;
	private static AsyncOperationHandle<SceneInstance> sceneHandle = null;
	private static bool loading = false;
	private static bool waiting = false;
	
	public static void StartGetInteractions() {
		coroutine1 = SceneHandler.ToggleZoneActiveAsync("Z04_10", true);
		loading = true;
		waiting = false;
	}
	
	public static void GetInteractions() {
		if (coroutine1 != null) {
			if (sceneHandle != null && sceneHandle.IsDone) {
				waiting = false;
				if (loading) {
					APScene = SceneManager.CreateScene("Archipelago");
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
					laptopData = APPickItem._ItemData;
					APItemData = UnityEngine.Object.Instantiate(laptopData).Cast<ItemData>();
					APPickItem._AutoDisable = false;
					APItemData._NameID = "Randomized Item";
					APItemData._DescriptionID = "A randomized item from Archipelago";
					sceneHandle = null;
				}
				else {
					sceneHandle = null;
				}
			}
			else if (waiting) {
				return;
			}
			else if (coroutine3 != null && coroutine3.MoveNext()) {
				if (coroutine3.Current is AsyncOperationHandle<SceneInstance>) {
					sceneHandle = coroutine3.Current.Cast<AsyncOperationHandle<SceneInstance>>();
					waiting = true;
				}
			}
			else if (coroutine2 != null && coroutine2.MoveNext()) {
				coroutine3 = coroutine2.Current.Cast<Il2CppSystem.Collections.IEnumerator>();
			}
			else if (coroutine1.MoveNext()) {
				coroutine2 = coroutine1.Current.Cast<Il2CppSystem.Collections.IEnumerator>();
			}
			else if (coroutine2 != null && coroutine3 != null && sceneHandle == null) {
				if (loading) {
					coroutine1 = SceneHandler.ToggleZoneActiveAsync("Z04_10", false);
					coroutine2 = null;
					coroutine3 = null;
					loading = false;
				}
				else {
					coroutine1 = null;
					coroutine2 = null;
					coroutine3 = null;
				}
			}
		} 
		
		// For now I just want to check out the AssetBundles
		// Il2CppSystem.Collections.IEnumerable bundleList = 
		// 	AssetBundle.GetAllLoadedAssetBundles().Cast<Il2CppSystem.Collections.IEnumerable>();
		// foreach (Il2CppSystem.Object bundleObj in bundleList) {}
		// 	// AssetBundle bundle = bundleObj.Cast<AssetBundle>();
		// 	// Melon<Randomizer>.Logger.Msg("Bundle " + bundle.name);
		// 	// Melon<Randomizer>.Logger.Msg("Streamed Scene Bundle: " + bundle.isStreamedSceneAssetBundle.ToString());
		// 	// IEnumerable<string> assetNameList = bundle.GetAllAssetNames();
		// 	// foreach (string assetName in assetNameList) {
		// 	// 	Melon<Randomizer>.Logger.Msg("\t" + "Asset " + assetName);
		// 	// }
		// }
	}
	
	public static void FillItemData() {
		itemDictionary = new Dictionary<int, ItemData>();
		itemDictionary.Add(1, InventoryHandler.ItemDatabase.RawMaterials);
		itemDictionary.Add(2, InventoryHandler.ItemDatabase.Laurentium);
		itemDictionary.Add(3, InventoryHandler.ItemDatabase.VitalModules);
		itemDictionary.Add(4, InventoryHandler.ItemDatabase.LogicBlocks);
		itemDictionary.Add(5, (ItemData)InventoryHandler.ItemDatabase.MementoHorseshoeMagnet);
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