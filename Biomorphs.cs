using Il2CppPixelCrushers.DialogueSystem;

namespace BiomorphRandomizer;

public static class Biomorphs {
	
	public static void ShuffleFreeBiomorphs(string room) {
		// if entering room with a free biomorph and
		// a corresponding real biomorph hasn't happened yet,
		// move the free biomorph to a different room
	}
	
	public static void CreateFreeBiomorphs() {
		// read save data variables to determine which
		// free biomorphs to give, and then get the menu
		// to update and display the buttons
	}
	
	public static void RecordRealBiomorph() {
		// Increment an archipelago variable keeping track
		// of the "real" biomorph count
		// If it's the first one for that monster
		// and it's not on the location of the free one
		// (it shouldn't be but it's good to check anyway),
		// set the free one to false
	}
}