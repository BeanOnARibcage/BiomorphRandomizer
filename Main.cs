using MelonLoader;
using HarmonyLib;

namespace BiomorphRandomizer;

public class Randomizer : MelonMod {
	private int updateCounter = 0;
	
	public override void OnInitializeMelon() {
		LoggerInstance.Msg("Randomizer Mod was loaded");
		Preferences.CreatePreferences();
		Preferences.LoadPreferences();
		SessionTools.CreateSession();
	}
	
	public override void OnUpdate() {
		ItemGiver.GetInteractions();
		bool itemGiven;
		updateCounter++;
		if (updateCounter > 60) { // Checking for items every frame is probably not necessary
			itemGiven = SessionTools.CheckForAndReceiveItem();
			while (itemGiven) {
				itemGiven = SessionTools.CheckForAndReceiveItem();
			}
			updateCounter = 0;
		}
	}
}
