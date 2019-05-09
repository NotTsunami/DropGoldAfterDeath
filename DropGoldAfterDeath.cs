using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;

namespace DropGoldAfterDeath
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("github.com/exel80/DropGoldAfterDeath", "DropGoldAfterDeath", "1.0.3")]
    public class DropGoldAfterDeath : BaseUnityPlugin
    {
        public void Awake()
        {
            // Thanky you @Storm312 for showing me how this works.
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += (orig, self, damageInfo, victim, victimNetworkUser) =>
            {
                orig(self, damageInfo, victim, victimNetworkUser);
                CharacterBody component = victim.GetComponent<CharacterBody>();
                NetworkUser networkUser = Util.LookUpBodyNetworkUser(component);

                if (!networkUser && !isMultiplayer()) return;
                if (component.master.money > 0)
                {
                    // Return if player does have a extra life ~ @iDeathHD Thank you for bug report
                    if (component.master.inventory.GetItemCount(ItemIndex.ExtraLife) >= 1)
                        return;

                    // Get money & alive count
                    uint money = component.master.money;
                    List<CharacterMaster> aliveLists = aliveList(component.master);

                    // Return if there is no more alive player
                    if (aliveLists.Count < 1)
                        return;
                    
                    // Take the money and split it
                    component.master.money = 0;
                    splitMoney(aliveLists, money);

                    // Broadcast drop message<
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = string.Format("<color=#e2b00b>{0} gold</color> has been dropped! " +
                        "Alive player(s) received <color=#e2b00b>{1} gold</color> each!", money, (money / Convert.ToUInt32(aliveLists.Count)))
                    });
                }
            };
        }

        /// <summary>
        /// Return true if more then 1 player in-game
        /// </summary>
        private static bool isMultiplayer()
        {
            return PlayerCharacterMasterController.instances.Count > 1;
        }

        /// <summary>
        /// Split dead player money to alive players.
        /// </summary>
        /// <param name="money">Dead player total money</param>
        /// <param name="alive">Alive people count</param>
        private static void splitMoney(List<CharacterMaster> alive, uint money)
        {
            foreach (CharacterMaster player in alive)
                player.money += (money / Convert.ToUInt32(alive.Count));
        }

        /// <summary>
        /// Return List<CharacterMaster> including only alive player(s).
        /// </summary>
        private static List<CharacterMaster> aliveList(CharacterMaster victim)
        {
            List<CharacterMaster> players = new List<CharacterMaster>();

            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.isClient
                    && player.isConnected
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
