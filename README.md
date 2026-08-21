An Archipelago mod for Biomorph! At present it only randomizes five items in the intro, and it doesn't do some of the things an Archipelago client needs to do.
For example, the host and slotname are currently hard-coded as localhost:38281 and Harlo, and the client doesn't handle locations being checked
or items being received when it's not connected.

## Installation Instructions
These instructions are for the Windows Steam version of the game. I haven't tested compatibility with other versions.
1. Get [MelonLoader](https://github.com/LavaGang/MelonLoader.Installer), and follow the instructions to install it in your copy of Biomorph.
2. Make sure you have the .NET SDK. If you're on Windows, you probably already have it.
3. In the command line, run `dotnet package download Archipelago.MultiClient.Net@6.7.1`.
4. In the package that should've just downloaded to your working directory, navigate to `archipelago.multiclient.net/6.7.1/lib/net6.0` and find the file
`Archipelago.MultiClient.Net.dll`. Copy it to the `Mods` folder in your copy of Biomorph. (You may have to run the game once with MelonLoader installed before
that folder will appear.)
5. From this repository's releases page, download `BiomorphRandomizer.dll` and copy it into the `Mods` folder in your copy of Biomorph.

## Usage
Create an Archipelago world and host it locally, making sure your slotname is `Harlo`. The game will attempt to connect to the Archipelago world when
loading a save file. Make sure you stay connected to Archipelago whenever you plan to send or receive items, since I haven't made functionality to
handle offline activity yet. I also recommend connecting an Archipelago text client. Once you start the game, the five items you can obtain in the Core's Lab during the intro (excluding the Bruisers) will be randomized. The item screen will still show you the original item that was in that location, but
if you check your inventory, you should have the randomized item instead. If you have a text client connected, you can verify which item you should've received.

The mod does not currently send a goal completion flag.