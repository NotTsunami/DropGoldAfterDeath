# DropGoldOnDeath
A Risk of Rain 2 mod that splits your gold among remaining alive players. All players are required to have this mod to achieve the intended effect.

## Features
This mod that splits your gold among remaining alive players. This is an improved fork of the original [DropGoldAfterDeath](https://thunderstore.io/package/exel80/DropGoldAfterDeath/) mod, by [exel80](https://github.com/exel80), with the following changes:
- Adds in funny, randomized quips on each death message
- Better compatibility with existing mods
- Updated to work with the latest Risk of Rain 2 update as of anniversary update

## Installation
It is highly recommended to use [r2modman](https://thunderstore.io/package/ebkr/r2modman/) to install DropGoldOnDeath because it will set up everything for you! If you are installing manually, you will need to make a folder in `Risk of Rain 2\BepInEx\plugins` called `DropGoldOnDeath` and drop the contents of the zip into it. The Language folder must be included, otherwise the mod will fail to function correctly.

## Changelog
### Version 2.0.0
- R2API dependency has been removed, making ShowDeathCause vanilla-compatible!
- Language support has been added
    - If you would like to contribute a translation, please look at the section below!
- Subscribe to `onCharacterDeathGlobal` instead of hooking at runtime

### Version 1.2.1
- Stricter network compatibility (Everyone must now have the same version)
- Switched to local stripped libs instead of relying on game's installation
    - Fewer binaries are included as well now
- Updated for anniversary update

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

### Contributing a Translation
Thank you for your interest in contributing a translation! You can contribute a translation by following the steps below:
1. Make a folder in the `Language` folder with the ISO 639-1 code as the name. For example, if I were contributing a French translation, the correct path would be `Language/fr`.
2. Copy the `dgod_tokens.json` file from `Language/en` to your newly created folder.
3. Add your translations!

Anything in brackets ({}) or wrapped in <> does not need to be translated. For example, if you were translating the `DEATH_MESSAGE` token, which reads `<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}` as of version 2.0.0, you would only translate "gave everyone" and "from the grave!".

## Credits to the Original Author
[exel80](https://github.com/exel80)

[Skull Crossbones icon](https://icons8.com/icons/set/self-destruct-button--v1) by [Icons8](https://icons8.com).
