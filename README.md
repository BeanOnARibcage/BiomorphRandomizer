An Archipelago mod for Biomorph! It currently covers from the beginning of the game through the end of
the Dunes of Time.

## Installation Instructions
These instructions are for the Windows Steam version of the game. I haven't tested compatibility with other versions.
1. Get [MelonLoader](https://github.com/LavaGang/MelonLoader.Installer), and follow the instructions to install it in your copy of Biomorph.
2. From this repository's releases page, download `BiomorphRandomizer.dll` and `Archipelago.MultiClient.Net.dll` and copy them both into the `Mods` folder in your copy of Biomorph.
3. Run the game once and close it once you get to the title screen (it may take a while to open the first time).
This will generate the preferences you'll use to set your connection info.

## Usage
Before opening the game, go to your game files. Open `UserData/MelonPreferences.cfg`, find the section labeled
`[biomorph_connection_info]`, enter the connection info you want to use. If there is no password, leave it as `""`.
Save the file and start the game.

The game will attempt to connect to the Archipelago multiworld when loading a save, using the connection info you entered in the preferences file. The game will show you what randomized items you find and receive, but if you
want to use Archipelago commands like `!hint`, you'll need to use a text client.

The current goal is to open the door to the Athenaeum.

## Preferences
\[biomorph_connection_info\]

host: The host to connect to. Defaults to "archipelago.gg".

port: Which port to use when connecting to the host. Defaults to 38281.

slotname: The name of your player slot in the multiworld. Defaults to "Harlo".

password: The password to connect with, or no password if set to an empty string "" (the default).

\[biomorph_randomizer_options\]

enable: If this is false, the mod won't apply its patches, letting you play
the game without randomizing items or connecting to Archipelago. Defaults to true.