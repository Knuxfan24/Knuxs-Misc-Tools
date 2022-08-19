# Knux's Miscellaneous Tools
A small repository containing simple C# code that I hack together to either make some of the stuff I do easier or just to test some stuff. I also dump random reverse engineering stuff in various states of unfinished here too because why not?

I don't officially support any of the stuff in this repo. It's mostly here for my own archival. If you want to use any of this, you're on your own.

___

The stuff here heavily relies on [Marathon](https://github.com/Big-Endian-32/Marathon), specifically my [custom Ninja fork](https://github.com/Knuxfan24/Marathon/tree/ninja).

[HedgeLib#](https://github.com/Radfordhound/HedgeLib/tree/master) is needed for a couple of the Sonic Unleashed Wii SET functions.

[Prs.net](https://github.com/FraGag/prs.net) is needed for the compression and decompression of Sonic and the Secret Rings/Sonic and the Black Knight ONE archives.

All three projects are included as GitHub Submodules now, so a proper clone will pull them down with it.

___

Also included is a modified version of the already modified importvcolorobj script from Heroes Power Plant that has been modified to pull in some additional data to badly emulate instancing to help with work for the Wrath of Cortex NuScene files. X and Y rotation is missing as Max is fucking stupid when it comes to most things, so that data is easier to input by hand, inputting the Y rotation then the X rotation manually seems to work every time.

___

# Supported:

## Adventure2 (Sonic Adventure 2)

- [SET file](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Adventure2/SET.cs) reading (was used for potential Sonic '06 mods).

## CarZ (Big Rigs Engine)

- [Material Library](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/CarZ/MaterialLibrary.cs) reading, writing and MTL exporting.

- [SCO Model](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/CarZ/SCO.cs) reading, writing and OBJ exporting.

## Gods (Data Design Interactive Engine)

- [WAD](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Gods/WAD.cs) archive reading and extracting, alongside inaccurate writing.

## ProjectM (Metroid Other M)

- [Message Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/ProjectM/MessageTable.cs) reading and writing.

## RockX7 (Megaman X7)

- Basic [SET file](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/RockX7/SET.cs) reading.

## SWA (Sonic Unleashed HD)

- [ArcInfo](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/SWA/ArcInfo.cs) reading and writing.

## SWA_Wii (Sonic Unleashed SD)

- Uncompressed [ONE](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/SWA_Wii/ONE.cs) archive reading, writing, extracting and creating.

- Basic [SET file](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/SWA_Wii/SET.cs) reading and writing, alongside a poorly cobbled together [SonicGLVL](https://github.com/DarioSamo/libgens-sonicglvl) exporter and importer.

- Most basic implementation of [Path](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/SWA_Wii/PathSpline.cs) reading.

## SonicNext (Sonic The Hedgehog (2006))

- [Functions](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/SonicNext/Functions.cs) for importing a model as an XNO, removing an XNO's vertex colours, converting an FBX file's animation to an XNM (either a single file or a directory of files), merging two XNMs together, changing an XNM's framerate, importing a model as a stage collision mesh, retargeting animations from one XNO to another and enabling translucency on an XNO (either a single material or every material in the XNO). 

## SonicPortable (Sonic The Hedgehog 4)

- [AMB](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/SonicPortable/AMB.cs) archive reading and extracting.

## Storybook (Sonic and the Secret Rings/Sonic and the Black Knight)

- Basic [Visiblity Block](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/VisibilityTable.cs) reading and writing.

- [ONE](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/ONE.cs) archive reading, writing, extracting and creating.

- Unfinished [Path](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/PathSpline.cs) file reading and writing.

- Basic [Player Motion Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/PlayerMotion.cs) reading.

- Basic [SET file](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/SET.cs) reading and writing.

- [SETItems](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/SETItems.cs) reading and writing.

- [TXD](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Storybook/TXD.cs) texture archive reading, writing, extracting and creating (doesn't convert to or from Dolphin GVR textures).

## Westwood (Monopoly (1995))

- [Message Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/Westwood/Text.cs) reading and writing.

## WrathOfCortex (Crash Bandicoot: The Wrath of Cortex)

- Basic [Crate Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/WrathOfCortex/CrateTable.cs) reading and writing.

- [Entity Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/WrathOfCortex/EntityTable.cs) reading and writing.

- Incomplete [GameCube NuScene](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/WrathOfCortex/NuScene.cs) reading and writing, alongside a very hacked up implementation of OBJ exporting that breaks a lot of the standard and really needs a [custom MaxScript](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Binaries/importvcolorobj_v1.3.7_woc.ms) to import correctly.

- Basic [Visiblity Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/WrathOfCortex/VisibilityTable.cs) reading.

- [Wumpa Fruit Table](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/WrathOfCortex/WumpaTable.cs) reading and writing. This format can also be written in little endian for the Xbox and PlayStation 2 versions of the game.

- [Function](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/main/Knux's%20Misc%20Tools/WrathOfCortex/Functions.cs) to display the position, rotation and scale of an instance in an NuScene. Mainly done for manually inputting X and Y rotation values into 3DS Max after an OBJ import.

## Other

- [Function](https://github.com/Knuxfan24/Knuxs-Misc-Tools/blob/c318ab10e5b2ecc8654584656b86fabdaaa69e5b/Knux's%20Misc%20Tools/Helpers.cs#L92) to take a string and run it through Google Translate a specified number of times. Used for my various Lost In Translation mod experiments. (Requires a valid Google Cloud API key to be provided)
