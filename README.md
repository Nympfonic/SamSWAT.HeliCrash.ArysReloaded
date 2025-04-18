# HelicopterCrashSites

DayZ inspired

## Overview
Credits go to SamSWAT for the original creation of this mod! https://dev.sp-tarkov.com/SamSWAT/HelicopterCrashSites & https://hub.sp-tarkov.com/files/file/659-helicopter-crash-sites

## Overview

BepInEx plugin that will add random helicopter crash sites. The initial chance is 10%, but it can be adjusted in the configuration manager (`F12` key) or alternatively, if you've launched up the game at least once with this mod, you can find the `com.SamSWAT.HeliCrash.ArysReloaded.cfg` file in your `BepInEx/config/` folder and change value there.

If you were lucky, after loading into the raid you may find downed UH-60 Blackhawk by thick column of smoke, its position is random and choosed from `HeliCrashLocations.json` file, here you can add your own locations or delete them. There should be a container with loot at the rear of the helicopter, currently, because of some limitations, it's just a copy of what was generated for the airdrop so technically there's no way to alter what will be in the container without affecting airdrops.

## How to install

1. Download the latest release here: [link](https://hub.sp-tarkov.com/files/file/1804-samswat-s-helicopter-crash-sites-arys-reloaded/#versions) -OR- build from source (instructions below)
2. Extract the zip file and drop the `BepInEx` folder into your SPT install folder (overwrite/merge if asked by Windows)

## Preview

![preview](https://media.discordapp.net/attachments/417281262085210112/972622826160930866/Escape_from_Tarkov_2022.04.27-17.43_1.png)

## Requirements

- Visual Studio (.NET desktop workload) or JetBrains Rider
- .NET Standard 2.1
- A text editor (VSCode highly recommended)

## How to build from source

1. Download/clone this repository
2. Open `project/SamSWAT.HeliCrash/SamSWAT.HeliCrash.ArysReloaded.csproj` in a text editor (VSCode highly recommended)
3. Modify the project macros to suit your needs (read the comments!!!) and save
4. Extract `sikorsky_uh60_blackhawk.bundle` from `mod/SamSWAT.HeliCrash/Assets.7z` to `project/SamSWAT.HeliCrash/CopyToOutput`
5. VS2022 > File > Open solution > `SamSWAT.HeliCrash.sln`
6. VS2022 > Build > Rebuild solution (if your project macros are set up correctly, the built mod will be correctly copied to your SPT `BepInEx/plugins` folder)
7. Run SPT
