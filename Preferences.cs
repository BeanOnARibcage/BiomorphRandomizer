using MelonLoader;

public static class Preferences {
	public static MelonPreferences_Category ConnectionInfo;
	public static MelonPreferences_Entry<string> Host;
	public static MelonPreferences_Entry<int> Port;
	public static MelonPreferences_Entry<string> Password;
	public static MelonPreferences_Entry<string> Slotname;
	
	public static MelonPreferences_Category OtherOptions;
	public static MelonPreferences_Entry<bool> Enable;
	
	public static void CreatePreferences() {
		ConnectionInfo = MelonPreferences.CreateCategory("biomorph_connection_info", "Biomorph Connection Info");
		Host = ConnectionInfo.CreateEntry<string>("host", "archipelago.gg", "Host");
		Port = ConnectionInfo.CreateEntry<int>("port", 38281, "Port");
		Password = ConnectionInfo.CreateEntry<string>("password", "", "Password");
		Slotname = ConnectionInfo.CreateEntry<string>("slotname", "Harlo", "Slot Name");
		
		OtherOptions = MelonPreferences.CreateCategory("biomorph_randomizer_options", "Biomorph Randomizer Options");
		Enable = OtherOptions.CreateEntry<bool>("enable", true, "Enable Randomization");
	}
	
	public static void LoadPreferences() {
		MelonPreferences.Load();
	}
}