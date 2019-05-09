using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;

namespace DropGoldAfterDeath
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("github.com/exel80/DropGoldAfterDeath", "DropGoldAfterDeath", "1.0.2")]
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
                    // Get money & alive count
                    uint money = component.master.money;
                    uint playerCount = Convert.ToUInt16(PlayerCharacterMasterController.instances.Count);
                    List<CharacterMaster> aliveLists = aliveList(component.master);

                    // Return if there is no more alive player
                    if (aliveLists.Count < 1)
                        return;
                    // Return if player does have a extra life ~ @iDeathHD Thank you for bug report
                    if (component.master.inventory.GetItemCount(ItemIndex.ExtraLife) >= 1)
                        return;

                    // Take the money and split it
                    component.master.money = 0;
                    splitMoney(component.master, money, aliveLists);

                    // Broadcast drop message<
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = string.Format("<color=#e2b00b>{0} gold</color> has been dropped! " +
                        "Alive player(s) received <color=#e2b00b>{1} gold</color> each!", money, (money/Convert.ToUInt16(aliveLists.Count)))
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
        /// <param name="victim">Dead player</param>
        /// <param name="money">Dead player total money</param>
        /// <param name="alive">Alive people count</param>
        private static void splitMoney(CharacterMaster victim, uint money, List<CharacterMaster> alive)
        {
            foreach (CharacterMaster player in alive)
            {
                if (player.isClient
                    && player.isClient
                    && !player.Equals(victim))
                {
                    player.money += (money/Convert.ToUInt16(alive.Count));
                }
            }
        }

        /// <summary>
        /// Return aliveList (BUG: include just death people, remember filter it out)
        /// </summary>
        private static List<CharacterMaster> aliveList(CharacterMaster victim)
        {
            List<CharacterMaster> listOfBodies = new List<CharacterMaster>();
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.isClient
                    && player.isConnected
                    && player.master.alive
                    && !player.master.Equals(victim))
                {
                    listOfBodies.Add(player.master);
                }
            }
            return listOfBodies;
        }
    }
}
