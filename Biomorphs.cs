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
	
	public static BiomorphRewardHolder BiomorphRewards;
	private static Dictionary<string, string> currentFreeBiomorphs;
	// key = monster name; value = serialization data of free biomorph
	public static Dictionary<string, int> rewardsIndexByName;
	
	public static void SetAndFillBiomorphRewards(BiomorphRewardHolder newHolder) {
		Melon<Randomizer>.Logger.Msg("SetAndFillBiomorphRewards entered");
		if (SessionTools.CheckConnection()) {
			Melon<Randomizer>.Logger.Msg("SetAndFillBiomorphRewards exit 1");
			return; // This should only be executed before the data in the database has been modified
		}
		rewardsIndexByName = new Dictionary<string, int>();
		BiomorphRewards = newHolder;
		BiomorphRewards.BiomorphRewards = 
			new Il2CppSystem.Collections.Generic.List<BiomorphRewardData>();
		BiomorphRewardData reward, firstReward;
		ItemData itemData;
		string monsterName;
		
		foreach (MonsterData monster in InventoryHandler.ItemDatabase.BiomorphArsenal) {
			reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[0]._BiomorphReward)
				.Cast<BiomorphRewardData>();
			firstReward = reward;
			BiomorphRewards.Add(reward);
			monsterName = monster._TextTable.GetFieldTextForLanguage(monster._NameID, 1);
			rewardsIndexByName.Add(monsterName, BiomorphRewards.Count - 1);
			
			reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[1]._BiomorphReward)
				.Cast<BiomorphRewardData>();
			reward._Icon = firstReward.Icon;
			itemData = UnityEngine.Object.Instantiate(firstReward.ItemData).Cast<ItemData>();
			itemData._DescriptionID = reward.Reward + " for the " + itemData.Name + ".";
			itemData._NameID = itemData.Name + " Level 2";
			reward._ItemData = itemData;
			BiomorphRewards.Add(reward);
			
			reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[2]._BiomorphReward)
				.Cast<BiomorphRewardData>();
			itemData = UnityEngine.Object.Instantiate(firstReward.ItemData).Cast<ItemData>();
			itemData._DescriptionID = reward.Reward + " for the " + itemData.Name + ".";
			itemData._NameID = itemData.Name + " Level 3";
			if (Il2CppType.TypeFromPointer(reward.ObjectClass) == Il2CppType.Of<BiomorphRewardDataChip>()) {
				itemData._SpriteIcon = reward.Icon;
			}
			reward._ItemData = itemData;
			reward._Icon = firstReward.Icon;
			BiomorphRewards.Add(reward);
		}
		Melon<Randomizer>.Logger.Msg("SetAndFillBiomorphRewards exit 2");
	}
	
	public static void SetUpBiomorphLocations() {
		Melon<Randomizer>.Logger.Msg("SetUpBiomorphLocations entered");
		SetMonsterRewards(5, 2000); // Fubirang
		SetMonsterRewards(12, 2003); // Scarbyttle
		Melon<Randomizer>.Logger.Msg("SetUpBiomorphLocations exit");
	}
	
	private static void SetMonsterRewards(int monsterIndex, long firstId) {
		Melon<Randomizer>.Logger.Msg("SetMonsterRewards entered");
		MonsterData monster = InventoryHandler.ItemDatabase.BiomorphArsenal[monsterIndex];
		for (int i = 0; i < 3; i++) {
			if (LocationFinder.IsLocationIncluded(firstId + i)) {
				ItemData item = LocationFinder.ItemBeingFound(firstId + i, false);
				item = UnityEngine.Object.Instantiate(item).Cast<ItemData>();
				item._NameID = (firstId + i).ToString();
				monster.BiomorphRewards[i]._BiomorphReward._ItemData = item;
			}
		}
		Melon<Randomizer>.Logger.Msg("SetMonsterRewards exit");
	}
	
	public static void ShuffleFreeBiomorphs(string room) {
		Melon<Randomizer>.Logger.Msg("ShuffleFreeBiomorphs entered");
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
		Melon<Randomizer>.Logger.Msg("ShuffleFreeBiomorphs exit");
		return;
	}
	
	private static void shuffleFreeBiomorph(string monsterName, string dataInRoom, string alternateData) {
		Melon<Randomizer>.Logger.Msg("shuffleFreeBiomorph entered");
		if (currentFreeBiomorphs[monsterName] == dataInRoom &&
			!DialogueLua.GetVariable("Archipelago_" + monsterName + "_Any", false)) {
			DialogueLua.SetVariable(dataInRoom, 0);
			DialogueLua.SetVariable(alternateData, 2);
			currentFreeBiomorphs[monsterName] = alternateData;
		}
		Melon<Randomizer>.Logger.Msg("shuffleFreeBiomorph exit");
	}
	
	public static void CreateFreeBiomorphs() {
		Melon<Randomizer>.Logger.Msg("CreateFreeBiomorphs entered");
		// read save data variables to determine which
		// free biomorphs to give
		// (shuffle free biomorphs will also be called,
		// so it doesn't matter which room we're starting in)
		currentFreeBiomorphs = new Dictionary<string, string>();
		createFreeBiomorph("Fubirang", "SerializationData_MindBreak_Harpoon_15");
		createFreeBiomorph("Scarbyttle", "SerializationData_MindBreak_Spiky_17");
		Melon<Randomizer>.Logger.Msg("CreateFreeBiomorphs exit");
	}
	
	private static void createFreeBiomorph(string monsterName, string monsterSerializationData) {
		Melon<Randomizer>.Logger.Msg("createFreeBiomorph entered");
		if (!DialogueLua.GetVariable("Archipelago_" + monsterName + "_Any", false)) {
			DialogueLua.SetVariable(monsterSerializationData, 2);
			currentFreeBiomorphs[monsterName] = monsterSerializationData;
		} else {
			currentFreeBiomorphs[monsterName] = "";
		}
		Melon<Randomizer>.Logger.Msg("createFreeBiomorph exit");
	}
	
	public static bool ShouldApplyLoadedBiomorphRewards = false;
		
	public static void ApplyObtainedBiomorphRewards() {
		Melon<Randomizer>.Logger.Msg("Clearing defense dict");
		ProgressHandler.BiomorphRewardActorDataDefense.Clear();
		Melon<Randomizer>.Logger.Msg("Clearing arsenal list");
		ProgressHandler.BiomorphRewardArsenal.Clear();
		Melon<Randomizer>.Logger.Msg("Clearing attack dict");
		ProgressHandler.BiomorphRewardDamageTypesAttack.Clear();
		Melon<Randomizer>.Logger.Msg("Clearing charges dict");
		ProgressHandler.BiomorphRewardWeaponDataCharge.Clear();
		Melon<Randomizer>.Logger.Msg("Rewards list length: " + BiomorphRewards.Count.ToString());
		for (int i = 0; i < BiomorphRewards.Count; i++) {
			if (DialogueLua.GetVariable("Archipelago_Biomorphs_" + i.ToString("D2"), false)) {
				Apply(BiomorphRewards[i]);
			}
		}
		ShouldApplyLoadedBiomorphRewards = false;
		Melon<Randomizer>.Logger.Msg("ApplyObtainedBiomorphRewards exit");
	}
	
	public static void RecordBiomorph(string monsterName, string monsterSerializationData) {
		Melon<Randomizer>.Logger.Msg("RecordBiomorph entered");
		if (!currentFreeBiomorphs.ContainsKey(monsterName)) {
			Melon<Randomizer>.Logger.Msg("RecordBiomorph exit 1");
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
		Melon<Randomizer>.Logger.Msg("RecordBiomorph exit 2");
	}
	
	public static void Apply(BiomorphRewardData reward) {
		Melon<Randomizer>.Logger.Msg("Apply entered");
		reward._RewardID = "AP_" + reward._RewardID;
		reward.Apply();
		Melon<Randomizer>.Logger.Msg("Apply exit");
	}
	
	// private static List<int> biomorphStorage;
	
	public static bool IsBiomorphable(string monsterName) {
		Melon<Randomizer>.Logger.Msg("IsBiomorphable entered");
		bool ret = currentFreeBiomorphs.ContainsKey(monsterName);
		Melon<Randomizer>.Logger.Msg("IsBiomorphable exit");
		return ret;
	}
	
	public static bool IsMonsterUnlocked(string monsterName) {
		Melon<Randomizer>.Logger.Msg("IsMonsterUnlocked entered");
		int index = rewardsIndexByName[monsterName];
		bool ret = DialogueLua.GetVariable("Archipelago_Biomorphs_" + index.ToString("D2"), false);
		Melon<Randomizer>.Logger.Msg("IsBiomorphable exit");
		return ret;
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

[RegisterTypeInIl2Cpp]
public class BiomorphRewardHolder : MonoBehaviour {
	public Il2CppReferenceField<Il2CppSystem.Collections.Generic.List<BiomorphRewardData>>
		BiomorphRewardsIl2Cpp;
	
	[HideFromIl2Cpp]
	public Il2CppSystem.Collections.Generic.List<BiomorphRewardData> BiomorphRewards {
		get {
			Melon<Randomizer>.Logger.Msg("get_BiomorphRewards entered");
			var ret = BiomorphRewardsIl2Cpp.Value;
			Melon<Randomizer>.Logger.Msg("get_BiomorphRewards exit");
			return ret;
		} set {
			Melon<Randomizer>.Logger.Msg("set_BiomorphRewards entered");
			BiomorphRewardsIl2Cpp.Value = value;
			Melon<Randomizer>.Logger.Msg("set_BiomorphRewards exit");
		}
	}
	
	[HideFromIl2Cpp]
	public BiomorphRewardData this[int index] {
		get {
			Melon<Randomizer>.Logger.Msg("indexer get entered");
			BiomorphRewardData ret = BiomorphRewards[index];
			Melon<Randomizer>.Logger.Msg("indexer get exit");
			return ret;
		}
		set {
			Melon<Randomizer>.Logger.Msg("indexer set entered");
			BiomorphRewards[index] = value;
			Melon<Randomizer>.Logger.Msg("indexer set exit");
		}
	}
	
	[HideFromIl2Cpp]
	public void Add(BiomorphRewardData element) {
		Melon<Randomizer>.Logger.Msg("Add entered");
		BiomorphRewards.Add(element);
		Melon<Randomizer>.Logger.Msg("Add exit");
	}
	
	[HideFromIl2Cpp]
	public void Clear() {
		Melon<Randomizer>.Logger.Msg("Clear entered");
		BiomorphRewards.Clear();
		Melon<Randomizer>.Logger.Msg("Clear exit");
	}
	
	[HideFromIl2Cpp]
	public int Count {
		get {
			Melon<Randomizer>.Logger.Msg("get_Count entered");
			int ret = BiomorphRewards.Count;
			Melon<Randomizer>.Logger.Msg("get_Count exit");
			return ret;
		}
	}
} 