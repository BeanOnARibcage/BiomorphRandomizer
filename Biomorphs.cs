using Il2CppPixelCrushers.DialogueSystem;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System.Collections.Generic;
using Il2CppLDS.MindBreaker.Data;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.Framework.Actors;
using Il2CppInterop.Runtime;

namespace BiomorphRandomizer;

public static class Biomorphs {
	
	public static BiomorphRewardHolder BiomorphRewards;
	private static Dictionary<string, string> currentFreeBiomorphs;
	// key = monster name; value = serialization data of free biomorph
	
	public static void SetAndFillBiomorphRewards(BiomorphRewardHolder newHolder) {
		if (SessionTools.CheckConnection()) {
			return; // This should only be executed before the data in the database has been modified
		}
		BiomorphRewards = newHolder;
		BiomorphRewardData reward, firstReward;
		ItemData itemData;
		
		foreach (MonsterData monster in InventoryHandler.ItemDatabase.BiomorphArsenal) {
			reward = UnityEngine.Object.Instantiate(monster.BiomorphRewards[0]._BiomorphReward)
				.Cast<BiomorphRewardData>();
			firstReward = reward;
			BiomorphRewards.Add(reward);
			
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
			DialogueLua.SetVariable(dataInRoom, false);
			DialogueLua.SetVariable(alternateData, true);
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
			DialogueLua.SetVariable(monsterSerializationData, true);
			currentFreeBiomorphs[monsterName] = monsterSerializationData;
		} else {
			currentFreeBiomorphs[monsterName] = "";
		}
	}
	
	public static void RecordBiomorph(long id) {
		string monsterName = LocationFinder.MonsterName(id);
		if (monsterName != null) {
			string variable = "Archipelago_" + monsterName + "_Any";
			if (!DialogueLua.GetVariable(variable, false)) {
				DialogueLua.SetVariable(variable, true);
				DialogueLua.SetVariable(currentFreeBiomorphs[monsterName], false);
				currentFreeBiomorphs[monsterName] = "";
			}
		} else {
			MelonLoader.Melon<Randomizer>.Logger.Msg(
				"Attempted to record biomorph with invalid id " + id.ToString());
		}
	}
	
	public static void Apply(BiomorphRewardData reward) {
		reward._RewardID = "AP_" + reward._RewardID;
		reward.Apply();
	}
}

[MelonLoader.RegisterTypeInIl2Cpp]
public class BiomorphRewardHolder : MonoBehaviour {
	public Il2CppSystem.Collections.Generic.List<BiomorphRewardData> BiomorphRewards;
	public BiomorphRewardData this[int index] {
		get { return BiomorphRewards[index]; }
		set { BiomorphRewards[index] = value; }
	}
	public void Add(BiomorphRewardData element) {
		BiomorphRewards.Add(element);
	}
	public void Clear() {
		BiomorphRewards.Clear();
	}
} 