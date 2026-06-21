using HarmonyLib;
using System.Threading.Tasks;
using Il2CppLDS.Sardonyx.Actions;
using MelonLoader;

namespace BiomorphRandomizer;

public class Patches {
	
	[HarmonyPatch(typeof(InteractionPickItem), nameof(InteractionPickItem.ExecutePickItem))]
	[HarmonyPrefix]
	static void PickItem(InteractionPickItem __instance) {
		// Figure out which location was picked up
		// Send the location to Archipelago
		//__instance._ItemData = null; //this crashes the game (need to figure out what to do instead)
		Melon<Randomizer>.Logger.Msg(__instance.name);
		return;
	}
}