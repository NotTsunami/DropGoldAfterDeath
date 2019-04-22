using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;

namespace DropGoldAfterDeath
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("github.com/exel80/DropGoldAfterDeath", "DropGoldAfterDeath", "0.1")]
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
                    uint alive = Convert.ToUInt16(PlayerCharacterMasterController.instances.Count - 1);

                    // Return if only 1 left
                    if (alive < 1) return;

                    // Take the money and split it
                    component.master.money = 0;
                    splitMoney(component.master, money, alive);

                    // Broadcast drop message
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = string.Format("<color=#e2b00b>{0} gold</color> has been dropped! Alive player(s) received <color=#e2b00b>{1} gold</color> each!", money, (money/alive))
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
        private static void splitMoney(CharacterMaster victim, uint money, uint alive)
        {
            foreach (CharacterMaster player in aliveList())
            {
                if (player.isClient
                    && player.isClient
                    && !player.Equals(victim))
                {
                    player.money += (money/alive);
                }
            }
        }

        /// <summary>
        /// Return aliveList (BUG: include just death people, remember filter it out)
        /// </summary>
        private static List<CharacterMaster> aliveList()
        {
            List<CharacterMaster> listOfBodies = new List<CharacterMaster>();
            foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
            {
                if (player.isClient
                    && player.isConnected
                    && player.master.alive)
                {
                    listOfBodies.Add(player.master);
                    //Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    //{
                    //    baseToken = string.Format("(ALIVE) DEBUG:{0}", player.master.name)
                    //});
                }
            }
            return listOfBodies;
        }
    }
}
