using MelonLoader;

public static class Preferences {
	public static MelonPreferences_Category ConnectionInfo;
	public static MelonPreferences_Entry<string> Host;
	public static MelonPreferences_Entry<int> Port;
	public static MelonPreferences_Entry<string> Password;
	public static MelonPreferences_Entry<string> Slotname;
	
	public static void CreatePreferences() {
		ConnectionInfo = MelonPreferences.CreateCategory("biomorph_connection_info", "Biomorph Connection Info");
		Host = ConnectionInfo.CreateEntry<string>("host", "archipelago.gg", "Host");
		Port = ConnectionInfo.CreateEntry<int>("port", 38281, "Port");
		Password = ConnectionInfo.CreateEntry<string>("password", null, "Password");
		Slotname = ConnectionInfo.CreateEntry<string>("slotname", "Harlo", "Slot Name");
	}
	
	public static void LoadPreferences() {
		MelonPreferences.Load();
	}
}