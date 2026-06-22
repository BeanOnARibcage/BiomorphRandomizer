using HarmonyLib;
using System.Threading.Tasks;
using Il2CppLDS.Sardonyx.Actions;
using MelonLoader;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	//ActionPickItem.StartExecution is called once per pickup, so that's a good one to patch
	[HarmonyPatch(typeof(ActionPickItem), nameof(ActionPickItem.StartExecution))]
	[HarmonyPrefix]
	static void PickItem(ActionPickItem __instance) {
		// Figure out which location was picked up
		// Send the location to Archipelago
		//__instance._ItemData = null; //this crashes the game (need to figure out what to do instead)
		Melon<Randomizer>.Logger.Msg(__instance.Interaction.name);
		return;
	}
}