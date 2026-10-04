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
			// this.HarmonyInstance.PatchAll(typeof(Loggers));
		}
	}
	
	public override void OnUpdate() {
		if (!Preferences.Enable.Value) return;
		if (SessionTools.LocationScouts == null && SessionTools.ScoutTask != null) {
			SessionTools.ReceiveLocationScouts();
		}
		bool checkAgain = false;
		updateCounter++;
		if (updateCounter > 30) { // Checking for items every frame is probably not necessary
			if (Biomorphs.ShouldApplyLoadedBiomorphRewards && ItemGiver.CanGetItem()) {
				LoggerInstance.Msg("About to apply loaded biomorph rewards");
				try {
					LoggerInstance.Msg("Entered try block");
					Biomorphs.ApplyObtainedBiomorphRewards();
					LoggerInstance.Msg("ApplyObtainedBiomorphRewards returned");
				} catch (Exception e) {
					LoggerInstance.Msg("Displaying stack trace:");
					LoggerInstance.Msg(e.StackTrace);
					throw;
				}
				LoggerInstance.Msg("Biomorph rewards applied without an exception");
			} else if (Patches.WaitingInteraction != null && ItemGiver.CanGetItem()) {
				Patches.WaitingInteraction.ExecutePickItem();
				Patches.WaitingInteraction = null;
			} else if (SessionTools.CheckConnection()) {	
				checkAgain = SessionTools.CheckForAndReceiveItem();
				LocationFinder.CheckForGoal();
			} else {
				SessionTools.Reconnect();
			}
			if (!checkAgain) {
				updateCounter = 0;
			}
		}
	}
}