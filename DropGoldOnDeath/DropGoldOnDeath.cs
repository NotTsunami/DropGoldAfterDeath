using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using Zio;
using Zio.FileSystems;

namespace DropGoldOnDeath
{
    // Support BiggerBazaar
    [BepInDependency("com.MagnusMagnuson.BiggerBazaar", BepInDependency.DependencyFlags.SoftDependency)]

    // Do not support ShareSuite
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInPlugin("dev.tsunami.DropGoldOnDeath", "DropGoldOnDeath", "2.0.0")]
    public class DropGoldOnDeath : BaseUnityPlugin
    {
        /// Array of funny strings to randomly pick from and append to chat message
        public static readonly string[] Quips = {"QUIP_1", "QUIP_2", "QUIP_3", "QUIP_4", "QUIP_5", "QUIP_6"};

        // FileSystem for ZIO
        public static FileSystem FileSystem { get; private set; }

        public void Awake()
        {
            // This adds in support for multiple languages
            // R2API offers LanguageAPI but we want to remain compatible with vanilla, thus use ZIO
            PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
            var assemblyDir = System.IO.Path.GetDirectoryName(Info.Location);
            FileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(assemblyDir));

            if (FileSystem.DirectoryExists("/Language/"))
            {
                Language.collectLanguageRootFolders += delegate (List<DirectoryEntry> list)
                {
                    list.Add(FileSystem.GetDirectoryEntry("/Language/"));
                };
            }

            // Subscribe to the pre-existing event, we were being a bad boy and hooking onto the GlobalEventManager before
            GlobalEventManager.onCharacterDeathGlobal += (damageReport) =>
            {
                CharacterBody component = damageReport.victimBody;
                NetworkUser networkUser = component.master.playerCharacterMasterController.networkUser;

                // Bail early if user is not in multiplayer
                if (!networkUser || !IsMultiplayer()) return;

                // Bail if ShareSuite is detected
                // ShareSuite uses a shared pool of gold already, thus defeating the purpose.
                if (HasShareSuite())
                {
                    UnityEngine.Debug.Log("DropGoldOnDeath: ShareSuite detected, no changes to gold");
                    return;
                }

                // Bail if BiggerBazaar is detected and a Newt Altar has been activated
                // For more context, BiggerBazaar allows you to retain your money after the stage when a
                // newt altar is active. In this case, don't split the gold among everyone and allow the
                // dead players to spend the money they did have.
                if (HasBiggerBazaar() && IsNewtAltarActive()) return;

                // Bail if the user has an unused Dio's Best Friend
                if (component.master.inventory.GetItemCount(RoR2Content.Items.ExtraLife) > 0) return;

                if (component.master.money > 0)
                {
                    // Pick a random quip to add a little humor
                    Xoroshiro128Plus rand = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
                    int index = rand.RangeInt(0, Quips.Length);

                    // Get players alive and gold count
                    List<CharacterMaster> aliveLists = AliveList(component.master);
                    uint money = component.master.money;
                    uint count = Convert.ToUInt32(aliveLists.Count);

                    // Return if there is no more alive players
                    if (count < 1) return;

                    // Zero the victim's gold and distribute it
                    uint split = money / count;
                    component.master.money = 0;
                    foreach (CharacterMaster player in aliveLists)
                    {
                        player.money += split;
                    }

                    // Broadcast drop message
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = $"DEATH_MESSAGE",
                        paramTokens = new [] { networkUser.userName, split.ToString(), Language.GetString(Quips[index]) }
                    });
                }
            };
        }

        /// Return true if more then 1 player in-game
        private static bool IsMultiplayer()
        {
            return PlayerCharacterMasterController.instances.Count > 1;
        }

        /// Return true if a Newt Altar has been activated
        private static bool IsNewtAltarActive()
        {
            return TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal;
        }

        /// Return true if BiggerBazaar is detected
        private static bool HasBiggerBazaar()
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.MagnusMagnuson.BiggerBazaar");
        }

        /// Return true if ShareSuite is detected
        private static bool HasShareSuite()
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite");
        }

        /// Return list of alive player(s)
        private static List<CharacterMaster> AliveList(CharacterMaster victim)
        {
            List<CharacterMaster> players = new List<CharacterMaster>();

            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.isConnected
                    && player.master.hasBody
                    && !player.master.Equals(victim))
                {
                    players.Add(player.master);
                }
            }

            return players;
        }
    }
}
