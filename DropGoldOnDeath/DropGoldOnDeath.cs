using BepInEx;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;

namespace DropGoldOnDeath
{
    // Support BiggerBazaar
    [BepInDependency("com.MagnusMagnuson.BiggerBazaar", BepInDependency.DependencyFlags.SoftDependency)]

    // Do not support ShareSuite
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("dev.tsunami.DropGoldOnDeath", "DropGoldOnDeath", "1.2.1")]
    public class DropGoldOnDeath : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += (orig, self, damageReport, networkUser) =>
            {
                orig(self, damageReport, networkUser);
                CharacterBody component = damageReport.victimBody;

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
                    uint count = Convert.ToUInt32(aliveLists.Count);
                    uint money = component.master.money;

                    // Return if there is no more alive players
                    if (count < 1) return;

                    // Zero the victim's gold and distribute it
                    component.master.money = 0;
                    SplitMoney(aliveLists, money, count);

                    // Broadcast drop message
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = $"<color=#00FF80>{networkUser.userName}</color> gave everyone <color=#e2b00b>{(money / count)} gold</color> from the grave! {Quips[index]}"
                    });
                }
            };
        }

        /// <summary>
        /// Array of funny strings to randomly pick from and append to chat message
        /// </summary>
        private static readonly string[] Quips = {"Don't forget to thank them!", "Everybody point and laugh!", "How kind of them!", "Maybe next time...", "Someone didn't heal enough!", "What were they thinking?!", "We're rich!"};

        /// <summary>
        /// Return true if more then 1 player in-game
        /// </summary>
        private static bool IsMultiplayer()
        {
            return PlayerCharacterMasterController.instances.Count > 1;
        }

        /// <summary>
        /// Return true if a Newt Altar has been activated
        /// </summary>
        private static bool IsNewtAltarActive()
        {
            return TeleporterInteraction.instance.shouldAttemptToSpawnShopPortal;
        }

        /// <summary>
        /// Return true if BiggerBazaar is detected
        /// </summary>
        private static bool HasBiggerBazaar()
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.MagnusMagnuson.BiggerBazaar");
        }

        /// <summary>
        /// Return true if ShareSuite is detected
        /// </summary>
        private static bool HasShareSuite()
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite");
        }

        /// <summary>
        /// Split dead player gold to alive players
        /// </summary>
        /// <param name="alive">List representing players alive</param>
        /// <param name="money">uint representing gold total of victim</param>
        /// <param name="count">uint representing players alive</param>
        private static void SplitMoney(List<CharacterMaster> alive, uint money, uint count)
        {
            foreach (CharacterMaster player in alive)
                player.money += (money / count);
        }

        /// <summary>
        /// Return list of alive player(s)
        /// </summary>
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
