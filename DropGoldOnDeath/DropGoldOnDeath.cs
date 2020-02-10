using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;

namespace DropGoldOnDeath
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("dev.tsunami.DropGoldOnDeath", "DropGoldOnDeath", "1.0.4")]
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
                    // Get players alive and gold count
                    List<CharacterMaster> aliveLists = AliveList(component.master);
                    uint count = Convert.ToUInt32(aliveLists.Count);
                    uint money = component.master.money;

                    // Return if there is no more alive players
                    if (count < 1) return;

                    // Split gold
                    SplitMoney(aliveLists, money, count);

                    // Broadcast drop message
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = $"<color=#00FF80>{networkUser.userName}</color> gave everyone <color=#e2b00b>{(money / count)} gold</color> from the grave, how nice!"
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
                    && player.master.alive
                    && !player.master.Equals(victim))
                {
                    players.Add(player.master);
                }
            }

            return players;
        }
    }
}
