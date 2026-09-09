An Archipelago mod for Biomorph! It currently covers the beginning of the game, up until opening the door
to the rest of Mezzo Skyway.

## Installation Instructions
These instructions are for the Windows Steam version of the game. I haven't tested compatibility with other versions.
1. Get [MelonLoader](https://github.com/LavaGang/MelonLoader.Installer), and follow the instructions to install it in your copy of Biomorph.
2. Make sure you have the .NET SDK. If you're on Windows, you probably already have it.
3. In the command line, run `dotnet package download Archipelago.MultiClient.Net@6.7.1`.
4. In the package that should've just downloaded to your working directory, navigate to `archipelago.multiclient.net/6.7.1/lib/net6.0` and find the file
`Archipelago.MultiClient.Net.dll`. Copy it to the `Mods` folder in your copy of Biomorph.
5. From this repository's releases page, download `BiomorphRandomizer.dll` and copy it into the `Mods` folder in your copy of Biomorph.
6. Run the game once and close it once you get to the title screen (it may take a while to open the first time).
This will generate the preferences you'll use to set your connection info.

## Usage
Before opening the game, go to your game files. Open `UserData/MelonPreferences.cfg`, find the section labeled
`[biomorph_connection_info]`, enter the connection info you want to use. If there is no password, leave it as `""`.
Save the file and start the game.

The game will attempt to connect to the Archipelago multiworld when loading a save, using the connection info you entered in the preferences file. The game will show you what randomized items you find and receive, but if you
want to use Archipelago commands like `!hint`, you'll need to use a text client.

The current goal is to reach Blightmoor, complete Boyd's quest, and open the door at the end of Mezzo Skyway's first room.

## Preferences
\[biomorph_connection_info\]

host: The host to connect to. Defaults to "archipelago.gg".

port: Which port of the host's to connect to. Defaults to 38281.

slotname: The name of your player slot in the multiworld. Defaults to "Harlo".

password: The password to connect with, or no password if set to an empty string "" (the default).

\[biomorph_randomizer_options\]

enable: If this is false, the mod will unpatch itself when preferences are loaded, letting you play
the game without randomizing items or connecting to Archipelago. Defaults to true.