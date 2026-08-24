using Il2CppLDS.Framework.Inventory;
using Il2CppLDS.MindBreaker.Core;
using Il2CppLDS.MindBreaker.Data;

namespace BiomorphRandomizer;

public static class ItemGiver {
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