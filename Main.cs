using MelonLoader;
using HarmonyLib;

namespace BiomorphRandomizer;

public class Randomizer : MelonMod {
	public override void OnInitializeMelon() {
		LoggerInstance.Msg("Randomizer Mod was loaded");
		Preferences.CreatePreferences();
		Preferences.LoadPreferences();
		SessionTools.CreateSession();
		HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patches));
	}
	
	public override void OnUpdate() {
		SessionTools.CheckForAndReceiveItem();
	}
}
