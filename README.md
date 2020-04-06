# DropGoldOnDeath
A Risk of Rain 2 mod that splits your gold among remaining alive players.

## Features
This mod that splits your gold among remaining alive players. This is an improved fork of the original [DropGoldAfterDeath](https://thunderstore.io/package/exel80/DropGoldAfterDeath/) mod, by [exel80](https://github.com/exel80), with the following changes:
- Updated to work with the latest R2API as of publishing
- Trimmed output down to only 1 line of chat
- Fixed null/multiplayer checks
- Reduced calculations

## Installation
1. Install [BepInEx](https://thunderstore.io/package/bbepis/BepInExPack/) and [R2API](https://thunderstore.io/package/tristanmcpherson/R2API/).
2. Copy the included `DropGoldOnDeath.dll` into the resulting `C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins` folder.
3. Launch the game and enjoy! To remove you simply need to delete the `DropGoldOnDeath.dll` file.

## Changelog
### Version 1.0.6
- Artifacts update!

### Version 1.0.5
- Zero out victim's gold (Prevents victim from retaining gold when resurrect through other means, such as the ShrineOfDio mod)

[Skull Crossbones icon](https://icons8.com/icons/set/self-destruct-button--v1) by [Icons8](https://icons8.com).
