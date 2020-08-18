using BepInEx;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;

namespace DropGoldOnDeath
{
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.DifferentModVersionsAreOk)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("dev.tsunami.DropGoldOnDeath", "DropGoldOnDeath", "1.1.0")]
    public class DropGoldOnDeath : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += (orig, self, damageReport, networkUser) =>
            {
                orig(self, damageReport, networkUser);
                CharacterBody component = damageReport.victimBody;
                
                if (!networkUser || !IsMultiplayer()) return;
                if (component.master.inventory.GetItemCount(ItemIndex.ExtraLife) > 0) return;
                if (component.master.money > 0)
                {
                    // Pick a random quip to add a little humor
                    string[] quips =
                        {"Don't forget to thank them!", "Everybody point and laugh!", "How kind of them!", "Maybe next time...", "Someone didn't heal enough!", "What were they thinking?!", "We're rich!"};
                    Random rand = new Random();
                    int index = rand.Next(quips.Length);

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
                        baseToken = $"<color=#00FF80>{networkUser.userName}</color> gave everyone <color=#e2b00b>{(money / count)} gold</color> from the grave! {quips[index]}"
                    });
                }
            };
        }

        /// <summary>
        /// Return true if more then 1 player in-game
        /// </summary>
        private static bool IsMultiplayer()
        {
            return PlayerCharacterMasterController.instances.Count > 1;
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
