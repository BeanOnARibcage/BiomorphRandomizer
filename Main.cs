using MelonLoader;
using HarmonyLib;

namespace BiomorphRandomizer;

public class Randomizer : MelonMod {
	public override void OnInitializeMelon() {
		LoggerInstance.Msg("Randomizer Mod was loded");
		HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patches));
	}
}
