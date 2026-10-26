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
using Il2CppInterop.Common;
using MelonLoader.NativeUtils;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace BiomorphRandomizer;

public static unsafe class Patcher {
	private delegate IntPtr GFPFD(Delegate d);
	
	public static void DoThePatching() {
		IntPtr orig;
		IntPtr patch;
		GFPFD gfpfd = Marshal.GetFunctionPointerForDelegate;
		
		orig = getOrig(typeof(ActionPickItem), nameof(ActionPickItem.StartExecution));
		delegateAPI_SE = PickItem;
		patch = gfpfd(delegateAPI_SE);
		hookAPI_SE = new NativeHook<API_SEDelegate>(orig, patch);
		hookAPI_SE.Attach();
		
		orig = getOrig(typeof(Z03MeleeWeapon2CS), nameof(Z03MeleeWeapon2CS.PostSequenceInternal));
		delegateMW2 = ManageStartingWeapon;
		patch = gfpfd(delegateMW2);
		hookMW2 = new NativeHook<MW2Delegate>(orig, patch);
		hookMW2.Attach();
		
		orig = getOrig(typeof(Z03Beacon1CS), nameof(Z03Beacon1CS.PreSequenceInternal));
		delegateB1 = RemoveBruisers;
		patch = gfpfd(delegateB1);
		hookB1 = new NativeHook<B1Delegate>(orig, patch);
		hookB1.Attach();
		
		orig = getOrig(typeof(Z03CaptureCS), nameof(Z03CaptureCS.PostSequenceInternal));
		delegateCCS = BreakTube;
		patch = gfpfd(delegateCCS);
		hookCCS = new NativeHook<CCSDelegate>(orig, patch);
		hookCCS.Attach();
		
		orig = getOrig(typeof(Attributes), nameof(Attributes.OnEnable));
		delegateA_OE = BreakPath;
		patch = gfpfd(delegateA_OE);
		hookA_OE = new NativeHook<A_OEDelegate>(orig, patch);
		hookA_OE.Attach();
		
		orig = getOrig(typeof(BoydCS), nameof(BoydCS.PreSequenceInternal));
		delegateBoydPre = CacheBoydVariables;
		patch = gfpfd(delegateBoydPre);
		hookBoydPre = new NativeHook<BoydPreDelegate>(orig, patch);
		hookBoydPre.Attach();
		
		orig = getOrig(typeof(BoydCS), nameof(BoydCS.PostSequenceInternal));
		delegateBoydPost = TrackingCenterRewards;
		patch = gfpfd(delegateBoydPost);
		hookBoydPost = new NativeHook<BoydPostDelegate>(orig, patch);
		hookBoydPost.Attach();
		
		orig = getOrig(typeof(ActionInteractionShop), nameof(ActionInteractionShop.StartExecution));
		delegateAIS_SE = Shop;
		patch = gfpfd(delegateAIS_SE);
		hookAIS_SE = new NativeHook<AIS_SEDelegate>(orig, patch);
		hookAIS_SE.Attach();
		
		orig = getOrig(typeof(ShopScreen), nameof(ShopScreen.OnShopPurchaseSuccess));
		delegateOSPS = SendShopLocation;
		patch = gfpfd(delegateOSPS);
		hookOSPS = new NativeHook<OSPSDelegate>(orig, patch);
		hookOSPS.Attach();
		
		orig = getOrig(typeof(PersistentDataManager), nameof(PersistentDataManager.ApplySaveData));
		delegateASD = ReadArchipelagoData;
		patch = gfpfd(delegateASD);
		hookASD = new NativeHook<ASDDelegate>(orig, patch);
		hookASD.Attach();
		
		orig = getOrig(typeof(ActionCinematic), nameof(ActionCinematic.StartExecution));
		delegateAC_SE = DetermineWhichDialogue;
		patch = gfpfd(delegateAC_SE);
		hookAC_SE = new NativeHook<AC_SEDelegate>(orig, patch);
		hookAC_SE.Attach();
		
		orig = getOrig(typeof(SaveHandler), nameof(SaveHandler.SaveGame));
		delegateSave = RecordItemsProcessed;
		patch = gfpfd(delegateSave);
		hookSave = new NativeHook<SaveDelegate>(orig, patch);
		hookSave.Attach();
		
		orig = getOrig(typeof(PanelSwitcherUI), nameof(PanelSwitcherUI.UISwitchPanel));
		delegatePanel = GetInteractions;
		patch = gfpfd(delegatePanel);
		hookPanel = new NativeHook<PanelDelegate>(orig, patch);
		hookPanel.Attach();
		
		orig = getOrig(typeof(SceneHandler), nameof(SceneHandler.LoadMapsInternalAsync));
		delegateLMIA = SetVariables;
		patch = gfpfd(delegateLMIA);
		hookLMIA = new NativeHook<LMIADelegate>(orig, patch);
		hookLMIA.Attach();
		
		orig = getOrig(typeof(MindBreakArea), nameof(MindBreakArea.OnMindBreak));
		delegateMBA_OMB = RecordBiomorph;
		patch = gfpfd(delegateMBA_OMB);
		hookMBA_OMB = new NativeHook<MBA_OMBDelegate>(orig, patch);
		hookMBA_OMB.Attach();
		
		orig = getOrig(typeof(BiomorphRewardDataArsenal), nameof(BiomorphRewardDataArsenal.Apply));
		delegateArsenal = Arsenal;
		patch = gfpfd(delegateArsenal);
		hookArsenal = new NativeHook<ArsenalDelegate>(orig, patch);
		hookArsenal.Attach();
		
		orig = getOrig(typeof(BiomorphRewardDataAttack), nameof(BiomorphRewardDataAttack.Apply));
		delegateAttack = Attack;
		patch = gfpfd(delegateAttack);
		hookAttack = new NativeHook<AttackDelegate>(orig, patch);
		hookAttack.Attach();
		
		orig = getOrig(typeof(BiomorphRewardDataCharge), nameof(BiomorphRewardDataCharge.Apply));
		delegateCharge = Charge;
		patch = gfpfd(delegateCharge);
		hookCharge = new NativeHook<ChargeDelegate>(orig, patch);
		hookCharge.Attach();
		
		orig = getOrig(typeof(BiomorphRewardDataChip), nameof(BiomorphRewardDataChip.Apply));
		delegateChip = Chip;
		patch = gfpfd(delegateChip);
		hookChip = new NativeHook<ChipDelegate>(orig, patch);
		hookChip.Attach();
		
		orig = getOrig(typeof(BiomorphRewardDataDefense), nameof(BiomorphRewardDataDefense.Apply));
		delegateDefense = Defense;
		patch = gfpfd(delegateDefense);
		hookDefense = new NativeHook<DefenseDelegate>(orig, patch);
		hookDefense.Attach();
		
		orig = getOrig(typeof(ProgressHandler), nameof(ProgressHandler.OnMindBreak));
		delegatePH_OMB = ResyncArsenal;
		patch = gfpfd(delegatePH_OMB);
		hookPH_OMB = new NativeHook<PH_OMBDelegate>(orig, patch);
		hookPH_OMB.Attach();
		
		orig = getOrig(typeof(LevelUpUI), nameof(LevelUpUI.OnBiomorphReward));
		delegateLU_OBR = StopExtraNotification;
		patch = gfpfd(delegateLU_OBR);
		hookLU_OBR = new NativeHook<LU_OBRDelegate>(orig, patch);
		hookLU_OBR.Attach();
	}
	
	public static void UndoThePatching() {
		hookAPI_SE.Detach();
		hookMW2.Detach();
		hookB1.Detach();
		hookCCS.Detach();
		hookA_OE.Detach();
		hookBoydPre.Detach();
		hookBoydPost.Detach();
		hookAIS_SE.Detach();
		hookOSPS.Detach();
		hookASD.Detach();
		hookAC_SE.Detach();
		hookSave.Detach();
		hookPanel.Detach();
		hookLMIA.Detach();
		hookMBA_OMB.Detach();
		hookArsenal.Detach();
		hookAttack.Detach();
		hookCharge.Detach();
		hookChip.Detach();
		hookDefense.Detach();
		hookPH_OMB.Detach();
		hookLU_OBR.Detach();
	}
	
	private static IntPtr getOrig(Type type, string method) {
		IntPtr orig = *(IntPtr*) (IntPtr) Il2CppInteropUtils.
			GetIl2CppMethodInfoPointerFieldForGeneratedMethod(type.GetMethod(method)).GetValue(null);
		return orig;
	}
	
	private static T getManagedValue<T>(IntPtr unmanaged, bool isField, bool boxed) {
		return IL2CPP.PointerToValueGeneric<T>(unmanaged, isField, boxed);
	}
	
	// Prefix for ActionPickItem.StartExecution
	private delegate IntPtr API_SEDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<API_SEDelegate> hookAPI_SE;
	private static API_SEDelegate delegateAPI_SE;
	private static IntPtr PickItem(IntPtr instance, IntPtr methodInfo) {
		ActionPickItem action = getManagedValue<ActionPickItem>(instance, false, false);
		bool run = Patches.PickItem(action);
		if (run) {
			return hookAPI_SE.Trampoline(instance, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Prefix for Z03MeleeWeapon2CS.PostSequenceInternal
	private delegate IntPtr MW2Delegate(IntPtr instance, IntPtr action, IntPtr methodInfo);
	private static NativeHook<MW2Delegate> hookMW2;
	private static MW2Delegate delegateMW2;
	private static IntPtr ManageStartingWeapon(IntPtr instance, IntPtr methodInfo, IntPtr action) {
		Patches.ManageStartingWeapon();
		return hookMW2.Trampoline(instance, action, methodInfo);
	}
	
	// Prefix for Z03Beacon1CS.PreSequenceInternal
	private delegate IntPtr B1Delegate(IntPtr instance, IntPtr action, IntPtr methodInfo);
	private static NativeHook<B1Delegate> hookB1;
	private static B1Delegate delegateB1;
	private static IntPtr RemoveBruisers(IntPtr instance, IntPtr action, IntPtr methodInfo) {
		Patches.RemoveBruisers();
		return hookB1.Trampoline(instance, action, methodInfo);
	}
	
	// Postfix for Z03CaptureCS.PostSequenceInternal
	private delegate IntPtr CCSDelegate(IntPtr instance, IntPtr action, IntPtr methodInfo);
	private static NativeHook<CCSDelegate> hookCCS;
	private static CCSDelegate delegateCCS;
	private static IntPtr BreakTube(IntPtr instance, IntPtr action, IntPtr methodInfo) {
		IntPtr result = hookCCS.Trampoline(instance, action, methodInfo);
		Patches.BreakTube();
		return result;
	}
	
	// Postfix for Attributes.OnEnable
	private delegate IntPtr A_OEDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<A_OEDelegate> hookA_OE;
	private static A_OEDelegate delegateA_OE;
	private static IntPtr BreakPath(IntPtr instance, IntPtr methodInfo) {
		IntPtr result = hookA_OE.Trampoline(instance, methodInfo);
		Attributes attributes = getManagedValue<Attributes>(instance, false, false);
		Patches.BreakPath(attributes);
		return result;
	}
	
	// Prefix for BoydCS.PreSequenceInternal
	private delegate IntPtr BoydPreDelegate(IntPtr instance, IntPtr action, IntPtr methodInfo);
	private static NativeHook<BoydPreDelegate> hookBoydPre;
	private static BoydPreDelegate delegateBoydPre;
	private static IntPtr CacheBoydVariables(IntPtr instance, IntPtr action, IntPtr methodInfo) {
		Patches.CacheBoydVariables();
		return hookBoydPre.Trampoline(instance, action, methodInfo);
	}
	
	// Prefix for BoydCS.PostSequenceInternal
	private delegate IntPtr BoydPostDelegate(IntPtr instance, IntPtr action, IntPtr methodInfo);
	private static NativeHook<BoydPostDelegate> hookBoydPost;
	private static BoydPostDelegate delegateBoydPost;
	private static IntPtr TrackingCenterRewards(IntPtr instance, IntPtr action, IntPtr methodInfo) {
		bool run = Patches.TrackingCenterRewards();
		if (run) {
			return hookBoydPost.Trampoline(instance, action, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Prefix for ActionInteractionShop.StartExecution
	private delegate IntPtr AIS_SEDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<AIS_SEDelegate> hookAIS_SE;
	private static AIS_SEDelegate delegateAIS_SE;
	private static IntPtr Shop(IntPtr instance, IntPtr methodInfo) {
		ActionInteractionShop shopAction = getManagedValue<ActionInteractionShop>(instance, false, false);
		Patches.Shop(shopAction);
		return hookAIS_SE.Trampoline(instance, methodInfo);
	}
	
	// Prefix for ShopScreen.OnShopPurchaseSuccess
	private delegate IntPtr OSPSDelegate(IntPtr instance, IntPtr shopItemData, IntPtr methodInfo);
	private static NativeHook<OSPSDelegate> hookOSPS;
	private static OSPSDelegate delegateOSPS;
	private static IntPtr SendShopLocation(IntPtr instance, IntPtr shopItemData, IntPtr methodInfo) {
		ShopItemData data = getManagedValue<ShopItemData>(shopItemData, false, false);
		ShopScreen screen = getManagedValue<ShopScreen>(instance, false, false);
		Patches.SendShopLocation(data, screen);
		return hookOSPS.Trampoline(instance, shopItemData, methodInfo);
	}
	
	// Postfix for PersistentDataManager.ApplySaveData
	private delegate IntPtr ASDDelegate(IntPtr instance, IntPtr saveData,
										IntPtr databaseResetOptions, IntPtr methodInfo);
	private static NativeHook<ASDDelegate> hookASD;
	private static ASDDelegate delegateASD;
	private static IntPtr ReadArchipelagoData(IntPtr instance, IntPtr saveData,
										IntPtr databaseResetOptions, IntPtr methodInfo) {
		IntPtr result = hookASD.Trampoline(instance, saveData, databaseResetOptions, methodInfo);
		Patches.ReadArchipelagoData();
		return result;
	}
	
	// Prefix for ActionCinematic.StartExecution
	private delegate IntPtr AC_SEDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<AC_SEDelegate> hookAC_SE;
	private static AC_SEDelegate delegateAC_SE;
	private static IntPtr DetermineWhichDialogue(IntPtr instance, IntPtr methodInfo) {
		ActionCinematic action = getManagedValue<ActionCinematic>(instance, false, false);
		Patches.DetermineWhichDialogue(action);
		return hookAC_SE.Trampoline(instance, methodInfo);
	}
	
	// Prefix for SaveHandler.SaveGame
	private delegate IntPtr SaveDelegate(IntPtr instance, IntPtr existingGame, IntPtr methodInfo);
	private static NativeHook<SaveDelegate> hookSave;
	private static SaveDelegate delegateSave;
	private static IntPtr RecordItemsProcessed(IntPtr instance, IntPtr existingGame, IntPtr methodInfo) {
		Patches.RecordItemsProcessed();
		return hookSave.Trampoline(instance, existingGame, methodInfo);
	}
	
	// Prefix for PanelSwitcherUI.UISwitchPanel
	private delegate IntPtr PanelDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<PanelDelegate> hookPanel;
	private static PanelDelegate delegatePanel;
	private static IntPtr GetInteractions(IntPtr instance, IntPtr methodInfo) {
		PanelSwitcherUI ui = getManagedValue<PanelSwitcherUI>(instance, false, false);
		Patches.GetInteractions(ui);
		return hookPanel.Trampoline(instance, methodInfo);
	}
	
	// Prefix for SceneHandler.LoadMapsInternalAsync
	private delegate IntPtr LMIADelegate(IntPtr instance, IntPtr zone, IntPtr methodInfo);
	private static NativeHook<LMIADelegate> hookLMIA;
	private static LMIADelegate delegateLMIA;
	private static IntPtr SetVariables(IntPtr instance, IntPtr zone, IntPtr methodInfo) {
		string zoneString = IL2CPP.Il2CppStringToManaged(zone);
		Patches.SetVariables(zoneString);
		return hookLMIA.Trampoline(instance, zone, methodInfo);
	}
	
	// Postfix for MindBreakArea.OnMindBreak
	private delegate IntPtr MBA_OMBDelegate(IntPtr instance, IntPtr monsterData, IntPtr position,
											IntPtr serializationDataMindBreak, IntPtr methodInfo);
	private static NativeHook<MBA_OMBDelegate> hookMBA_OMB;
	private static MBA_OMBDelegate delegateMBA_OMB;
	private static IntPtr RecordBiomorph(IntPtr instance, IntPtr monsterData, IntPtr position,
										IntPtr serializationDataMindBreak, IntPtr methodInfo) {
		MonsterData monster = getManagedValue<MonsterData>(monsterData, false, false);
		SerializationData variable = getManagedValue<SerializationData>(serializationDataMindBreak,
			false, false);
		IntPtr result = hookMBA_OMB.Trampoline(instance, monsterData, position,
			serializationDataMindBreak, methodInfo);
		Patches.RecordBiomorph(monster, variable);
		return result;
	}
	
	// Prefix for BiomorphRewardDataArsenal.Apply
	private delegate IntPtr ArsenalDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<ArsenalDelegate> hookArsenal;
	private static ArsenalDelegate delegateArsenal;
	private static IntPtr Arsenal(IntPtr instance, IntPtr methodInfo) {
		BiomorphRewardDataArsenal data = getManagedValue<BiomorphRewardDataArsenal>(instance, false, false);
		bool run = Patches.Arsenal(data);
		if (run) {
			return hookArsenal.Trampoline(instance, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Prefix for BiomorphRewardDataAttack.Apply
	private delegate IntPtr AttackDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<AttackDelegate> hookAttack;
	private static AttackDelegate delegateAttack;
	private static IntPtr Attack(IntPtr instance, IntPtr methodInfo) {
		BiomorphRewardDataAttack data = getManagedValue<BiomorphRewardDataAttack>(instance, false, false);
		bool run = Patches.Attack(data);
		if (run) {
			return hookAttack.Trampoline(instance, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Prefix for BiomorphRewardDataCharge.Apply
	private delegate IntPtr ChargeDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<ChargeDelegate> hookCharge;
	private static ChargeDelegate delegateCharge;
	private static IntPtr Charge(IntPtr instance, IntPtr methodInfo) {
		BiomorphRewardDataCharge data = getManagedValue<BiomorphRewardDataCharge>(instance, false, false);
		bool run = Patches.Charge(data);
		if (run) {
			return hookCharge.Trampoline(instance, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Prefix for BiomorphRewardDataChip.Apply
	private delegate IntPtr ChipDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<ChipDelegate> hookChip;
	private static ChipDelegate delegateChip;
	private static IntPtr Chip(IntPtr instance, IntPtr methodInfo) {
		BiomorphRewardDataChip data = getManagedValue<BiomorphRewardDataChip>(instance, false, false);
		bool run = Patches.Chip(data);
		if (run) {
			return hookChip.Trampoline(instance, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Prefix for BiomorphRewardDataDefense.Apply
	private delegate IntPtr DefenseDelegate(IntPtr instance, IntPtr methodInfo);
	private static NativeHook<DefenseDelegate> hookDefense;
	private static DefenseDelegate delegateDefense;
	private static IntPtr Defense(IntPtr instance, IntPtr methodInfo) {
		BiomorphRewardDataDefense data = getManagedValue<BiomorphRewardDataDefense>(instance, false, false);
		bool run = Patches.Defense(data);
		if (run) {
			return hookDefense.Trampoline(instance, methodInfo);
		} else {
			return IntPtr.Zero;
		}
	}
	
	// Postfix for ProgressHandler.OnMindBreak
	private delegate IntPtr PH_OMBDelegate(IntPtr instance, IntPtr monsterData, IntPtr origin,
											IntPtr serializationData, IntPtr methodInfo);
	private static NativeHook<PH_OMBDelegate> hookPH_OMB;
	private static PH_OMBDelegate delegatePH_OMB;
	private static IntPtr ResyncArsenal(IntPtr instance, IntPtr monsterData, IntPtr origin,
										IntPtr serializationData, IntPtr methodInfo) {
		MonsterData monster = getManagedValue<MonsterData>(monsterData, false, false);
		SerializationData variable = getManagedValue<SerializationData>(serializationData, false, false);
		IntPtr result = hookPH_OMB.Trampoline(instance, monsterData, origin, serializationData, methodInfo);
		Patches.ResyncArsenal(monster, variable);
		return result;
	}
	
	// Prefix for LevelUpUI.OnBiomorphReward
	private delegate IntPtr LU_OBRDelegate(IntPtr instance, IntPtr monsterData,
										IntPtr rewardUnlocked, IntPtr methodInfo);
	private static NativeHook<LU_OBRDelegate> hookLU_OBR;
	private static LU_OBRDelegate delegateLU_OBR;
	private static IntPtr StopExtraNotification(IntPtr instance, IntPtr monsterData,
											IntPtr rewardUnlocked, IntPtr methodInfo) {
		MonsterData monster = getManagedValue<MonsterData>(monsterData, false, false);
		bool leaveRewardUnlocked = Patches.StopExtraNotification(monster);
		if (!leaveRewardUnlocked) {
			rewardUnlocked = (IntPtr)NativeBool.False;
		}
		return hookLU_OBR.Trampoline(instance, monsterData, rewardUnlocked, methodInfo);
	}
	
	private enum NativeBool {
		False = 0,
		True = 1
	}
}

public static class Patches {
	private static void log(string message) {
		Melon<Randomizer>.Logger.Msg(message);
	}
	
	public static InteractionPickItem WaitingInteraction = null;
			
	// ActionPickItem.StartExecution is called once per pickup, so that's a good one to patch
	public static bool PickItem(ActionPickItem __instance) {
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
		return true;
	}
	
	public static void ManageStartingWeapon() {
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
	
	public static void RemoveBruisers() {
		if (!ItemGiver.BruisersReceived) {
			ItemData bruisers = ItemGiver.GetItemData(418, false);
			InventoryHandler.UpdateItem(bruisers, 0);
		}
	}
	
	public static void BreakTube() {
		GameObject tubeGO = GameObject.Find("/Root/States/Prefab_GO_Z03_FerroxVatCS");
		Attributes tubeAttributes = tubeGO.GetComponent<Attributes>();
		tubeAttributes.Health = 1;
		// May need Health=0 for certain starting weapons
	}

	public static void BreakPath(Attributes __instance) {
		if (__instance.name == "Prefab_Destructible_Path_z03_01(Clone)" &&
		ProgressHandler.BeaconActivated > 0) {
			__instance.Health = 0;
			return;
		}
	}
	
	private static bool shop2Finished = false, shop3Finished = false;

	public static void CacheBoydVariables() {
		shop2Finished = DialogueLua.GetVariable("NPCs.Boyd_Shop2_Finished", false);
		shop3Finished = DialogueLua.GetVariable("NPCs.Boyd_Shop3_Finished", false);
	}

	public static bool TrackingCenterRewards() {
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
				WaitingInteraction = interaction;
				SessionTools.SendLocation(54);
				return false;
			}
		}
		return true;
	}

	private static Dictionary<string, List<int>> shopPrices = new Dictionary<string, List<int>>();
	
	public static void Shop(ActionInteractionShop __instance) {
		int priceReductions = 0;
		if (SessionTools.SlotData != null) {
			priceReductions = (int)(long)SessionTools.SlotData["reduce_shop_prices"];
		}
		InteractionShop interaction = __instance.Interaction;
		if (interaction._ShopNameID == "UI_CityRegistry") {
			// Special handling to stop the player from building the laboratory early
			// Since that prevents the conversation where Meed gives you the blueprint from happening
			if (interaction.Items[9].UnlockConditions.Count < 2) {
				SerializationData extraCondition = new SerializationData();
				extraCondition._VariableName = "Quest.SQ35_State3_Completed";
				extraCondition.name = "Extra_Archipelago_Condition";
				interaction.Items[9].UnlockConditions.Add(extraCondition);
			}
			// Same thing for Salm & Alma's shop
			if (interaction.Items[13].UnlockConditions.Count < 2) {
				SerializationData extraCondition = new SerializationData();
				extraCondition._VariableName = "Quest.SQ01_State1_Completed";
				extraCondition.name = "Extra_Archipelago_Condition";
				interaction.Items[13].UnlockConditions.Add(extraCondition);
			}
		} else {
			string shop = interaction._ShopNameID + "_" + SceneHandler.CurrentZoneName;
			bool newShop = false;
			if (!shopPrices.ContainsKey(shop)) {
				newShop = true;
				shopPrices[shop] = new List<int>();
			}
			ShopItemData shopItemData;
			for (int i = 0; i < interaction.Items.Count; i++) {
				shopItemData = interaction.Items[i];
				if (newShop) {
					if (priceReductions == 2) {
						shopItemData._Money /= 5;
					} else if (priceReductions == 1) {
						if (shop == "UI_SlarShop_Z00_12" || 
								interaction._ShopNameID == "UI_AsrarMarket" && i < 3) {
							shopItemData._Money /= 5;
						}
					}
					shopPrices[shop].Add(shopItemData._Money);
				} else {
					shopItemData._Money = shopPrices[shop][i];
				}
				long id = LocationFinder.IdFromSerializationData(shopItemData.VariableName);
				if (id < 0) {
					continue;
				}
				shopItemData._ItemData = LocationFinder.ItemBeingFound(id, false);
			}
		}
	}

	public static void SendShopLocation(ShopItemData shopItemData, ShopScreen __instance) {
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

	public static void ReadArchipelagoData() {
		log("PersistentDataManager.ApplySaveData postfix entered");
		SessionTools.ItemsProcessed = DialogueLua.GetVariable("Archipelago_Items", 0);
		ItemGiver.ScargatoCount = DialogueLua.GetVariable("Game.ScargatosFound", 0);
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
			Dictionary<long, int> progressiveItems = TableHandler.TableToDictInt(
				DialogueLua.GetVariable("Archipelago_Progressive_Items").asTable.luaTable);
			log("Loading:");
			foreach (KeyValuePair<long, int> pair in progressiveItems) {
				log(pair.Key.ToString() + ": " + pair.Value.ToString());
			}
			ItemGiver.FillProgressiveItems(progressiveItems);
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

	public static void DetermineWhichDialogue(ActionCinematic __instance) {
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
		if (cinematicType == Il2CppType.Of<Z01MQ03CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z01MQ03CS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<SalmKertarCS>()) {
			Quests.HandleCinematic(cinematic.Cast<SalmKertarCS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<Z01SQ06CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z01SQ06CS>());
			return;
		}
		if (cinematicType == Il2CppType.Of<Z00SQ06CS>()) {
			Quests.HandleCinematic(cinematic.Cast<Z00SQ06CS>());
			return;
		}
		return;
	}	

	public static void RecordItemsProcessed() {
		Melon<Randomizer>.Logger.Msg("SaveGame called");
		DialogueLua.SetVariable("Archipelago_Items", SessionTools.ItemsProcessed);
		DialogueLua.SetVariable("Game.ScargatosFound", ItemGiver.ScargatoCount);
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
		log("Saving:");
		foreach (KeyValuePair<long, int> pair in ItemGiver.ProgressiveItems) {
			log(pair.Key.ToString() + ": " + pair.Value.ToString());
		}
		DialogueLua.SetVariable("Archipelago_Progressive_Items",
			TableHandler.DictToTable(ItemGiver.ProgressiveItems));
	}

	public static void GetInteractions(PanelSwitcherUI __instance) {
		if (__instance.name != "Start Game Button") {
			return;
		}
		if (ItemGiver.APItemData != null) {
			return;
		}
		ItemGiver.MakeAPInteraction();
		Biomorphs.SetAndFillBiomorphRewards();
		LocationFinder.FillLocationDictionary();
	}

	public static void SetVariables(string zone) {
		Quests.SetQuestVariables(zone);
		Biomorphs.ShuffleFreeBiomorphs(zone);
	}

	public static void RecordBiomorph(MonsterData monsterData, SerializationData serializationDataMindBreak) {
		if (serializationDataMindBreak == null || monsterData == null) {
			return; // This method is also called when turning back into Harlo
		}
		Biomorphs.RecordBiomorph(monsterData._TextTable.GetFieldTextForLanguage(monsterData._NameID, 1),
			serializationDataMindBreak.VariableName);
	}
	
	private static bool PrefixForApply(BiomorphRewardData instance) {
		long locationId;
		if (instance._RewardID.StartsWith("AP_")) { //code that I'm the one who called the method
			instance._RewardID = instance._RewardID.Substring(3);
			return true;
		} else {
			if (GameManager.GameState == GameManager.EGameState.LoadGame ||
				GameManager.GameState == GameManager.EGameState.ReturnToSAFE ||
				GameManager.GameState == GameManager.EGameState.Dead) {
				return false; // Don't let the game apply rewards as it's initializing
			}
			locationId = Biomorphs.FindBiomorphLocationId(instance);
			if (locationId == -1) {
				return true; // Unrandomized reward
			}
			ItemData item = LocationFinder.ItemBeingFound(locationId, true);
			SessionTools.SendLocation(locationId);
			LocationFinder.RecordLocationSerializationData(locationId);
			InteractionPickItem interaction = 
				UnityEngine.Object.Instantiate(ItemGiver.APPickItem).Cast<InteractionPickItem>();
			interaction._ItemData = item;
			WaitingInteraction = interaction;
			return false;
		}
	}

	public static bool Arsenal(BiomorphRewardDataArsenal __instance) {
		return PrefixForApply(__instance);
	}

	public static bool Attack(BiomorphRewardDataAttack __instance) {
		return PrefixForApply(__instance);
	}

	public static bool Charge(BiomorphRewardDataCharge __instance) {
		return PrefixForApply(__instance);
	}

	public static bool Chip(BiomorphRewardDataChip __instance) {
		return PrefixForApply(__instance);
	}

	public static bool Defense(BiomorphRewardDataDefense __instance) {
		return PrefixForApply(__instance);
	}

	public static void ResyncArsenal(MonsterData monsterData, SerializationData serializationData) {
		if (serializationData == null || monsterData == null || monsterData._NameID == ""
			|| monsterData._TextTable == null) {
			if (monsterData != null) log(monsterData.Name);
			return;
		}
		string monsterName = Biomorphs.MonsterName(monsterData);
		if (Biomorphs.IsBiomorphable(monsterName)) {
			// Biomorphs.RestoreBiomorphs(biomorphsStem);
			// biomorphsStem = null;
			// Biomorphs.RestoreFreeBiomorph(monsterName);
			ProgressHandler.BiomorphRewardArsenal.Clear();
			foreach (MonsterData monster in Biomorphs.AllUnlockedMonsters()) {
				ProgressHandler.BiomorphRewardArsenal.Add(monster);
			}
		}
	}

	// returns true: use original value of rewardUnlocked
	// returns false: use rewardUnlocked = false
	public static bool StopExtraNotification(MonsterData monsterData) {
		if (monsterData == null) {
			log("monster was null");
			return true;
		}
		if (Biomorphs.IsBiomorphable(Biomorphs.MonsterName(monsterData))) {
			log("monster was biomorphable");
			return false;
		} else {
			log("monster was not biomorphable");
			return true;
		}
	}
}

public static class Loggers {
}