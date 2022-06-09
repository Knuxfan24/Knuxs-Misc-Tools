# Knux's Miscellaneous Tools
A small repository containing simple C# code that I hack together to either make some of the stuff I do easier or just to test some stuff. I also dump random reverse engineering stuff in various states of unfinished here too because why not?

The stuff here heavily relies on [Marathon](https://github.com/Big-Endian-32/Marathon), specifically my [custom Ninja fork](https://github.com/Knuxfan24/Marathon/tree/ninja).

[HedgeLib#](https://github.com/Radfordhound/HedgeLib/tree/master) is needed for a couple of the Sonic Unleashed Wii SET functions.

[Prs.net](https://github.com/FraGag/prs.net) is needed for the compression and decompression of Sonic and the Secret Rings/Sonic and the Black Knight ONE archives.

All three projects are included as GitHub Submodules now, so a proper clone will pull them down with it.

Also included is a modified version of the already modified importvcolorobj script from Heroes Power Plant that has been modified to pull in some additional data to badly emulate instancing. X and Y rotation is missing as Max is fucking stupid when it comes to most things, so that data is easier to input by hand, inputting the Y rotation then the X rotation manually seems to work every time.