# DropGoldOnDeath
A Risk of Rain 2 mod that splits your gold among remaining alive players.

## Features
This mod that splits your gold among remaining alive players. This is an improved fork of the original [DropGoldAfterDeath](https://thunderstore.io/package/exel80/DropGoldAfterDeath/) mod, by [exel80](https://github.com/exel80), with the following changes:
- Adds in funny, randomized quips on each death message
- Better compatibility with existing mods
- Updated to work with the latest Risk of Rain 2 update as of anniversary update

## Installation
It is highly recommended to use [r2modman](https://thunderstore.io/package/ebkr/r2modman/) to install DropGoldOnDeath because it will set up everything for you! If you are installing manually, you will need to make a folder in `Risk of Rain 2\BepInEx\plugins` called `DropGoldOnDeath` and drop the contents of the zip into it.

## Changelog
### Version 2.0.0
- R2API dependency has been removed, making ShowDeathCause vanilla-compatible!
- Subscribe to `onCharacterDeathGlobal` instead of hooking at runtime
- Language support has been added
    - If you would like to contribute a translation, please look at the section below!
- Configuration has been added
    - A configurable gold multiplier has been added that is applied to the pool of gold split between alive players
    - Quips can now be disabled

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

## Contributing a Translation
Thank you for your interest in contributing a translation! You can contribute a translation by following the steps below:

1. Locate this code block in `DropGoldOnDeath.cs` (I have omitted several of the `list.Add()` calls as there are a lot of tokens, but the [...] block refers to all of the tokens):
```csharp
Language.onCurrentLanguageChanged += () =>
{
    var list = new List<KeyValuePair<string, string>>();
    if (Language.currentLanguageName == "en")
    {
            list.Add(new KeyValuePair<string, string>("DGOD_DEATH_MESSAGE", "<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}"));
            [...]
            list.Add(new KeyValuePair<string, string>("QUIP_6", "What were they thinking?!"));
    }
    Language.currentLanguage.SetStringsByTokens(list);
}
```
2. You will want to add another branch to the if statement that checks against your language's [IETF language tag](https://en.wikipedia.org/wiki/IETF_language_tag). The code should now look like the following if I were translating for French:
```csharp
Language.onCurrentLanguageChanged += () =>
{
    var list = new List<KeyValuePair<string, string>>();
    if (Language.currentLanguageName == "en")
    {
            list.Add(new KeyValuePair<string, string>("DGOD_DEATH_MESSAGE", "<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}"));
            [...]
            list.Add(new KeyValuePair<string, string>("QUIP_6", "What were they thinking?!"));
    }
    else if (Language.currentLanguageName == "fr")
    {
    
    }
    Language.currentLanguage.SetStringsByTokens(list);
}
```
3. Copy all of the `list.add();` calls from the `en` section to your branch, and then translate them as per the suggestion below. The final code should look like:
```csharp
Language.onCurrentLanguageChanged += () =>
{
    var list = new List<KeyValuePair<string, string>>();
    if (Language.currentLanguageName == "en")
    {
            list.Add(new KeyValuePair<string, string>("DGOD_DEATH_MESSAGE", "<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}"));
            [...]
            list.Add(new KeyValuePair<string, string>("QUIP_6", "What were they thinking?!"));
    }
    else if (Language.currentLanguageName == "fr")
    {
		    list.Add(new KeyValuePair<string, string>("DGOD_DEATH_MESSAGE", "<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}"));
            [...]
            list.Add(new KeyValuePair<string, string>("QUIP_6", "What were they thinking?!"));
    }
    Language.currentLanguage.SetStringsByTokens(list);
}
```
4. Send in a pull request through GitHub!

Anything in brackets ({}) or wrapped in <> does not need to be translated. For example, if you were translating the `DGOD_DEATH_MESSAGE` token, which reads `<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}` as of version 2.0.0, you would only translate "gave everyone" and "from the grave!".

## Credits to the Original Author
[exel80](https://github.com/exel80)

[Skull Crossbones icon](https://icons8.com/icons/set/self-destruct-button--v1) by [Icons8](https://icons8.com).
