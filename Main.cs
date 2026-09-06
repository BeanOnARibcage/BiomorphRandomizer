using MelonLoader;
using HarmonyLib;

namespace BiomorphRandomizer;

public class Randomizer : MelonMod {
	private int updateCounter = 0;
	
	public override void OnInitializeMelon() {
		LoggerInstance.Msg("Randomizer Mod was loaded");
		Preferences.CreatePreferences();
		Preferences.LoadPreferences();
		if (Preferences.Enable.Value) {
			SessionTools.CreateSession();
		} else {
			LoggerInstance.Msg("Randomizer disabled. Unpatching methods.");
			this.HarmonyInstance.UnpatchSelf();
		}
	}
	
	public override void OnUpdate() {
		if (!Preferences.Enable.Value) return;
		ItemGiver.GetInteractions();
		bool itemGiven;
		updateCounter++;
		if (updateCounter > 60) { // Checking for items every frame is probably not necessary
			if (SessionTools.CheckConnection()) {	
				itemGiven = SessionTools.CheckForAndReceiveItem();
				while (itemGiven) {
					itemGiven = SessionTools.CheckForAndReceiveItem();
				}
				LocationFinder.CheckForGoal();
			} else {
				SessionTools.Reconnect();
			}
			updateCounter = 0;
		}
	}
}
