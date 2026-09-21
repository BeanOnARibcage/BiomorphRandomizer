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
		if (SessionTools.CheckConnection()) {
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
	}
	
	public static void SetUpBiomorphLocations() {
		SetMonsterRewards(5, 2000); // Fubirang
		SetMonsterRewards(12, 2003); // Scarbyttle
	}
	
	private static void SetMonsterRewards(int monsterIndex, long firstId) {
		MonsterData monster = InventoryHandler.ItemDatabase.BiomorphArsenal[monsterIndex];
		for (int i = 0; i < 3; i++) {
			if (LocationFinder.IsLocationIncluded(firstId + i)) {
				ItemData item = LocationFinder.ItemBeingFound(firstId + i, false);
				item = UnityEngine.Object.Instantiate(item).Cast<ItemData>();
				item._NameID = (firstId + i).ToString();
				monster.BiomorphRewards[i]._BiomorphReward._ItemData = item;
			}
		}
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
	
	// private static List<int> biomorphStorage;
	
	public static bool IsBiomorphable(string monsterName) {
		return currentFreeBiomorphs.ContainsKey(monsterName);
	}
	
	public static bool IsMonsterUnlocked(string monsterName) {
		int index = rewardsIndexByName[monsterName];
		return DialogueLua.GetVariable("Archipelago_Biomorphs_" + index.ToString("D2"), false);
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
			return BiomorphRewardsIl2Cpp.Value;
		} set {
			BiomorphRewardsIl2Cpp.Value = value;
		}
	}
	
	[HideFromIl2Cpp]
	public BiomorphRewardData this[int index] {
		get { return BiomorphRewards[index]; }
		set { BiomorphRewards[index] = value; }
	}
	
	[HideFromIl2Cpp]
	public void Add(BiomorphRewardData element) {
		BiomorphRewards.Add(element);
	}
	
	[HideFromIl2Cpp]
	public void Clear() {
		BiomorphRewards.Clear();
	}
	
	[HideFromIl2Cpp]
	public int Count {
		get {
			return BiomorphRewards.Count;
		}
	}
} 