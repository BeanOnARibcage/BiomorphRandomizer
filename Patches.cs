using HarmonyLib;
using System.Threading.Tasks;
using Il2CppLDS.Sardonyx.Actions;
using MelonLoader;
using Il2CppLDS.Sardonyx.Actors;

namespace BiomorphRandomizer;

[HarmonyPatch]
public class Patches {
	//ActionPickItem.StartExecution is called once (or twice?) per pickup, so that's a good one to patch
	[HarmonyPatch(typeof(ActionPickItem), nameof(ActionPickItem.StartExecution))]
	[HarmonyPrefix]
	static void PickItem(ActionPickItem __instance) {
		InteractionPickItem interaction = __instance.Interaction;
		if (interaction.name == "Prefab_Interaction_Money(Clone)") {
			return;
		}
		int id = LocationFinder.IntroIdFromInteractionName(interaction.name);
		if (id < 0) {
			return;
		}
		SessionTools.SendLocation(id);
		interaction.ItemQuantity = 0;
		//Melon<Randomizer>.Logger.Msg(__instance.Interaction.name);
		return;
	}
	
	[HarmonyPatch(typeof(ActorPlayer), nameof(ActorPlayer.OnEnable))]
	[HarmonyPostfix]
	static void Connect() {
		if (!SessionTools.CheckConnection()) {
			SessionTools.Connect();
		}
	}
}