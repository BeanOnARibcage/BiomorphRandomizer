using Il2CppPixelCrushers.DialogueSystem;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System.Collections.Generic;
using Il2CppLDS.MindBreaker.Data;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.Framework.Actors;
using Il2CppInterop.Runtime;
using MelonLoader;
using Il2CppInterop.Runtime.Attributes;

namespace BiomorphRandomizer;

public static class Biomorphs {
	private static Dictionary<string, string> currentFreeBiomorphs;
	// key = monster name; value = serialization data of free biomorph
	public static Dictionary<string, int> rewardsIndexByName;
	
	public static void SetAndFillBiomorphRewards() {
		// if (SessionTools.CheckConnection()) {
		// 	return; // This should only be executed before the data in the database has been modified
		// }
		rewardsIndexByName = new Dictionary<string, int>();
		// BiomorphRewardData reward, firstReward;
		// ItemData itemData;
		string monsterName;
		int arsenalIndex = 0;
		
		foreach (MonsterData monster in InventoryHandler.ItemDatabase.BiomorphArsenal) {
			// reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[0]._BiomorphReward)
			// 	.Cast<BiomorphRewardData>();
			// firstReward = reward;
			// BiomorphRewards.Add(reward);
			
			// These lines are the only part I want to keep
			monsterName = monster._TextTable.GetFieldTextForLanguage(monster._NameID, 1);
			rewardsIndexByName.Add(monsterName, arsenalIndex);
			arsenalIndex += 3;
			
			// reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[1]._BiomorphReward)
			// 	.Cast<BiomorphRewardData>();
			// reward._Icon = firstReward.Icon;
			// itemData = UnityEngine.Object.Instantiate(firstReward.ItemData).Cast<ItemData>();
			// itemData._DescriptionID = reward.Reward + " for the " + itemData.Name + ".";
			// itemData._NameID = itemData.Name + " Level 2";
			// reward._ItemData = itemData;
			// BiomorphRewards.Add(reward);
			
			// reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[2]._BiomorphReward)
			// 	.Cast<BiomorphRewardData>();
			// itemData = UnityEngine.Object.Instantiate(firstReward.ItemData).Cast<ItemData>();
			// itemData._DescriptionID = reward.Reward + " for the " + itemData.Name + ".";
			// itemData._NameID = itemData.Name + " Level 3";
			// if (Il2CppType.TypeFromPointer(reward.ObjectClass) == Il2CppType.Of<BiomorphRewardDataChip>()) {
			// 	itemData._SpriteIcon = reward.Icon;
			// }
			// reward._ItemData = itemData;
			// reward._Icon = firstReward.Icon;
			// BiomorphRewards.Add(reward);
		}
	}
	
	public static void SetUpBiomorphLocations() {
	// 	SetMonsterRewards(5, 2000); // Fubirang
	// 	SetMonsterRewards(12, 2003); // Scarbyttle
	// }
	
	// private static void SetMonsterRewards(int monsterIndex, long firstId) {
	// 	MonsterData monster = InventoryHandler.ItemDatabase.BiomorphArsenal[monsterIndex];
	// 	for (int i = 0; i < 3; i++) {
	// 		if (LocationFinder.IsLocationIncluded(firstId + i)) {
	// 			ItemData item = LocationFinder.ItemBeingFound(firstId + i, false);
	// 			item = UnityEngine.Object.Instantiate(item).Cast<ItemData>();
	// 			item._NameID = (firstId + i).ToString();
	// 			monster.BiomorphRewards[i]._BiomorphReward._ItemData = item;
	// 		}
	// 	}
	}
	
	public static void ShuffleFreeBiomorphs(string room) {
		// not shuffle as in randomization, just moving them out of the way
		switch (room) {
			case "Z04_20":
				shuffleFreeBiomorph("Fubirang", "SerializationData_MindBreak_Harpoon_15",
					"SerializationData_MindBreak_Harpoon_13");
				break;
			case "Z04_19":
				shuffleFreeBiomorph("Fubirang", "SerializationData_MindBreak_Harpoon_13",
					"SerializationData_MindBreak_Harpoon_15");
				break;
			case "Z04_21":
				shuffleFreeBiomorph("Scarbyttle", "SerializationData_MindBreak_Spiky_17",
					"SerializationData_MindBreak_Spiky_14");
				break;
			case "Z04_18":
				shuffleFreeBiomorph("Scarbyttle", "SerializationData_MindBreak_Spiky_14",
					"SerializationData_MindBreak_Spiky_17");
				break;
		}
		return;
	}
	
	private static void shuffleFreeBiomorph(string monsterName, string dataInRoom, string alternateData) {
		if (currentFreeBiomorphs[monsterName] == dataInRoom &&
			!DialogueLua.GetVariable("Archipelago_" + monsterName + "_Any", false)) {
			DialogueLua.SetVariable(dataInRoom, 0);
			DialogueLua.SetVariable(alternateData, 2);
			currentFreeBiomorphs[monsterName] = alternateData;
		}
	}
	
	public static void CreateFreeBiomorphs() {
		// read save data variables to determine which
		// free biomorphs to give
		// (shuffle free biomorphs will also be called,
		// so it doesn't matter which room we're starting in)
		currentFreeBiomorphs = new Dictionary<string, string>();
		createFreeBiomorph("Fubirang", "SerializationData_MindBreak_Harpoon_15");
		createFreeBiomorph("Scarbyttle", "SerializationData_MindBreak_Spiky_17");
	}
	
	private static void createFreeBiomorph(string monsterName, string monsterSerializationData) {
		if (!DialogueLua.GetVariable("Archipelago_" + monsterName + "_Any", false)) {
			DialogueLua.SetVariable(monsterSerializationData, 2);
			currentFreeBiomorphs[monsterName] = monsterSerializationData;
		} else {
			currentFreeBiomorphs[monsterName] = "";
		}
	}
	
	public static bool ShouldApplyLoadedBiomorphRewards = false;
		
	public static void ApplyObtainedBiomorphRewards() {
		ProgressHandler.BiomorphRewardActorDataDefense.Clear();
		ProgressHandler.BiomorphRewardArsenal.Clear();
		ProgressHandler.BiomorphRewardDamageTypesAttack.Clear();
		ProgressHandler.BiomorphRewardWeaponDataCharge.Clear();
		for (int i = 0; i < InventoryHandler.ItemDatabase.BiomorphArsenal.Count * 3; i++) {
			if (DialogueLua.GetVariable("Archipelago_Biomorphs_" + i.ToString("D2"), false)) {
				Apply(GetBiomorphRewardData(i));
			}
		}
		ShouldApplyLoadedBiomorphRewards = false;
	}
	
	public static void RecordBiomorph(string monsterName, string monsterSerializationData) {
		if (!currentFreeBiomorphs.ContainsKey(monsterName)) {
			return; // monster is not in scope of the randomizer yet
		}
		string variable = "Archipelago_" + monsterName + "_Any";
		if (!DialogueLua.GetVariable(variable, false)) {
			DialogueLua.SetVariable(variable, true);
			if (currentFreeBiomorphs[monsterName] != monsterSerializationData) {
				DialogueLua.SetVariable(currentFreeBiomorphs[monsterName], 0);
			}
			currentFreeBiomorphs[monsterName] = "";
		}
	}
	
	public static void Apply(BiomorphRewardData reward) {
		reward._RewardID = "AP_" + reward._RewardID;
		reward.Apply();
	}
	
	// Takes a BiomorphRewardData that the game is trying to apply and returns
	// the corresponding locaiton id
	public static long FindBiomorphLocationId(BiomorphRewardData reward) {
		MonsterData testMonster;
		int rewardIndex;
		testMonster = InventoryHandler.ItemDatabase.BiomorphArsenal[5]; // Fubirang
		rewardIndex = testRewardList(reward, testMonster);
		if (rewardIndex >= 0) {
			return 2000 + rewardIndex;
		}
		testMonster = InventoryHandler.ItemDatabase.BiomorphArsenal[12]; // Scarbyttle
		rewardIndex = testRewardList(reward, testMonster);
		if (rewardIndex >= 0) {
			return 2003 + rewardIndex;
		}
		return -1;
	}
		
	private static int testRewardList(BiomorphRewardData reward, MonsterData monster) {
		for (int i = 0; i < 3; i++) {
			if (monster.BiomorphRewards[i]._BiomorphReward == reward) {
				return i;
			}
		}
		return -1;
	}
	
	public static string MonsterName(MonsterData data) {
		if (data._TextTable == null) {
			return "";
		} else {
			return data._TextTable.GetFieldTextForLanguage(data._NameID, 1);
		}
	}
	
	public static bool IsBiomorphable(string monsterName) {
		return currentFreeBiomorphs.ContainsKey(monsterName);
	}
	
	public static bool IsMonsterUnlocked(string monsterName) {
		int index = rewardsIndexByName[monsterName];
		return DialogueLua.GetVariable("Archipelago_Biomorphs_" + index.ToString("D2"), false);
	}
	
	public static IEnumerable<MonsterData> AllUnlockedMonsters() {
		foreach (MonsterData monster in InventoryHandler.ItemDatabase.BiomorphArsenal) {
			string monsterName = monster._TextTable.GetFieldTextForLanguage(monster._NameID, 1);
			if (IsMonsterUnlocked(monsterName)) {
				yield return monster;
			}
		}
	}
	
	public static BiomorphRewardData GetBiomorphRewardData(int ones) {
		int monsterIndex, levelIndex;
		monsterIndex = Math.DivRem(ones, 3, out levelIndex);
		MonsterData monster = InventoryHandler.ItemDatabase.BiomorphArsenal[monsterIndex];
		BiomorphRewardData reward = monster.BiomorphRewards[levelIndex]._BiomorphReward;
		if (levelIndex >= 1 && reward.ItemData == null) {
			BiomorphRewardData firstReward = monster.BiomorphRewards[0]._BiomorphReward;
			reward._ItemData = UnityEngine.Object.Instantiate(
				firstReward.ItemData).Cast<ItemData>();
			reward.ItemData._DescriptionID = reward.Reward + " for the " + reward.ItemData.Name + ".";
			if (levelIndex == 1) {
				reward.ItemData._NameID = firstReward.Reward + " Level 2";
			} else {
				reward.ItemData._NameID = firstReward.Reward + " Level 3";
				if (Il2CppType.TypeFromPointer(reward.ObjectClass) == 
					Il2CppType.Of<BiomorphRewardDataChip>()) {
					reward.ItemData._SpriteIcon = reward.Icon;
				}
			}
		}
		return reward;
	}
	/*
	public static void TempRemoveBiomorphs(string serializationDataPrefix) {
		biomorphStorage = new List<int>();
		string data;
		for (int i = 1; i <= 25; i++) {
			data = serializationDataPrefix + i.ToString("D2");
			if (DialogueLua.GetVariable(data, 0) > 0) {
				biomorphStorage.Add(i);
				DialogueLua.SetVariable(data, 0);
			}
		}
	}
	
	public static void RestoreBiomorphs(string serializationDataPrefix) {
		if (biomorphStorage == null) {
			return;
		}
		string data;
		foreach (int i in biomorphStorage) {
			data = serializationDataPrefix + i.ToString("D2");
			DialogueLua.SetVariable(data, 2);
		}
		biomorphStorage = null;
	}
	*/
	/*
	public static void TempRemoveFreeBiomorph(string monsterName) {
		string variableName = "Archipelago_" + monsterName + "_Any";
		if (!currentFreeBiomorphs.ContainsKey(monsterName)) {
			return;
		}
		if (!DialogueLua.GetVariable(variableName, false)) {
			DialogueLua.SetVariable(currentFreeBiomorphs[monsterName], 0);
		}
	}
	
	public static void RestoreFreeBiomorph(string monsterName) {
		string variableName = "Archipelago_" + monsterName + "_Any";
		if (!currentFreeBiomorphs.ContainsKey(monsterName)) {
			return;
		}
		if (!DialogueLua.GetVariable(variableName, false)) {
			DialogueLua.SetVariable(currentFreeBiomorphs[monsterName], 2);
		}
	}
	*/
}

// new plan: extend ItemDatabase.BiomorphArsenal with copies of the MonsterData
// that have copies of the BiomorphRewardData.
// never mind; it's possible there are parts of the game code that iterate through that list
// having duplicate copies of each monster wouldn't be a good idea

// [RegisterTypeInIl2Cpp]
// public class BiomorphRewardHolder : MonoBehaviour {
// 	public Il2CppReferenceField<Il2CppSystem.Collections.Generic.List<BiomorphRewardData>>
// 		BiomorphRewardsIl2Cpp;
	
// 	[HideFromIl2Cpp]
// 	public Il2CppSystem.Collections.Generic.List<BiomorphRewardData> BiomorphRewards {
// 		get {
// 			Melon<Randomizer>.Logger.Msg("get_BiomorphRewards entered");
// 			var ret = BiomorphRewardsIl2Cpp.Value;
// 			Melon<Randomizer>.Logger.Msg("get_BiomorphRewards exit");
// 			return ret;
// 		} set {
// 			Melon<Randomizer>.Logger.Msg("set_BiomorphRewards entered");
// 			BiomorphRewardsIl2Cpp.Value = value;
// 			Melon<Randomizer>.Logger.Msg("set_BiomorphRewards exit");
// 		}
// 	}
	
// 	[HideFromIl2Cpp]
// 	public BiomorphRewardData this[int index] {
// 		get {
// 			Melon<Randomizer>.Logger.Msg("indexer get entered");
// 			BiomorphRewardData ret = BiomorphRewards[index];
// 			Melon<Randomizer>.Logger.Msg("indexer get exit");
// 			return ret;
// 		}
// 		set {
// 			Melon<Randomizer>.Logger.Msg("indexer set entered");
// 			BiomorphRewards[index] = value;
// 			Melon<Randomizer>.Logger.Msg("indexer set exit");
// 		}
// 	}
	
// 	[HideFromIl2Cpp]
// 	public void Add(BiomorphRewardData element) {
// 		Melon<Randomizer>.Logger.Msg("Add entered");
// 		BiomorphRewards.Add(element);
// 		Melon<Randomizer>.Logger.Msg("Add exit");
// 	}
	
// 	[HideFromIl2Cpp]
// 	public void Clear() {
// 		Melon<Randomizer>.Logger.Msg("Clear entered");
// 		BiomorphRewards.Clear();
// 		Melon<Randomizer>.Logger.Msg("Clear exit");
// 	}
	
// 	[HideFromIl2Cpp]
// 	public int Count {
// 		get {
// 			Melon<Randomizer>.Logger.Msg("get_Count entered");
// 			int ret = BiomorphRewards.Count;
// 			Melon<Randomizer>.Logger.Msg("get_Count exit");
// 			return ret;
// 		}
// 	}
// } 