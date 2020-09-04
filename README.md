# DropGoldOnDeath
A Risk of Rain 2 mod that splits your gold among remaining alive players. All players are required to have this mod to achieve the intended effect.

## Features
This mod that splits your gold among remaining alive players. This is an improved fork of the original [DropGoldAfterDeath](https://thunderstore.io/package/exel80/DropGoldAfterDeath/) mod, by [exel80](https://github.com/exel80), with the following changes:
- Adds in funny, randomized quips on each death message
- Better compatibility with existing mods
- Updated to work with the latest Risk of Rain 2 update as of publish date

## Installation
1. Install [BepInEx](https://thunderstore.io/package/bbepis/BepInExPack/) and [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/).
2. Copy the included `DropGoldOnDeath.dll` into the resulting `C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins` folder.
3. Launch the game and enjoy! To remove you simply need to delete the `DropGoldOnDeath.dll` file.

## Changelog
### Version 1.2.0
- Supports BiggerBazaar now! Gold is not dropped when the mod is detected and a Newt Altar is active.
- No longer attempts to activate if ShareSuite is detected.
- Updated to latest R2API.

### Version 1.1.0
- Updated for Risk of Rain 2 version 1.0
- Adds in funny quips to each death message

### Version 1.0.6
- Artifacts update!

### Version 1.0.5
- Zero out victim's gold (Prevents victim from retaining gold when resurrecting through other means, such as the ShrineOfDio mod)

[Skull Crossbones icon](https://icons8.com/icons/set/self-destruct-button--v1) by [Icons8](https://icons8.com).
