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
			Patcher.DoThePatching();
		} else {
			LoggerInstance.Msg("Randomizer disabled.");
		}
	}
	
	public override void OnDeinitializeMelon() {
		Patcher.UndoThePatching();
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
				Biomorphs.ApplyObtainedBiomorphRewards();
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