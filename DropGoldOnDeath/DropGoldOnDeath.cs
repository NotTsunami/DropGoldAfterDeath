using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global
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

        public void Awake()
        {
            var quipsEnabled =
                Config.Bind(new ConfigDefinition("Drop Gold On Death", "Quips Enabled"), true, new ConfigDescription("Enables quips, which are fun little messages appended to the message in chat after death.", null, Array.Empty<object>())).Value;
            var goldMultiplier = Config.Bind(
                new ConfigDefinition("Drop Gold On Death", "Gold Multiplier"), 1f,
                new ConfigDescription("Gold multiplier applied to gold split among players.", null,
                    Array.Empty<object>())).Value;

            // For some reason, we were unable to get languages to properly add via Zio, thus they are
            // added directly via hooking onto the onCurrentLanguageChanged event. This isn't awful,
            // but it does add some extra code that can be avoided. Pull requests that shift this to
            // Zio and local files are welcome.
            Language.onCurrentLanguageChanged += () =>
            {
                var list = new List<KeyValuePair<string, string>>();
                if (Language.currentLanguageName == "en")
                {
                    list.Add(new KeyValuePair<string, string>("DGOD_DEATH_MESSAGE", "<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color> from the grave! {2}"));
                    list.Add(new KeyValuePair<string, string>("DGOD_DEATH_MESSAGE_WITHOUT_QUIP", "<color=#00FF80>{0}</color> gave everyone <color=#e2b00b>{1} gold</color>!"));

                    list.Add(new KeyValuePair<string, string>("QUIP_1", "Don't forget to thank them for dying!"));
                    list.Add(new KeyValuePair<string, string>("QUIP_2", "Everybody point and laugh!"));
                    list.Add(new KeyValuePair<string, string>("QUIP_3", "How kind of them!"));
                    list.Add(new KeyValuePair<string, string>("QUIP_4", "Maybe next time..."));
                    list.Add(new KeyValuePair<string, string>("QUIP_5", "Someone needs more health LOL"));
                    list.Add(new KeyValuePair<string, string>("QUIP_6", "What were they thinking?!"));
                }
                Language.currentLanguage.SetStringsByTokens(list);
            };

            // Subscribe to the pre-existing event, we were being a bad boy and hooking onto the GlobalEventManager before
            GlobalEventManager.onCharacterDeathGlobal += (damageReport) =>
            {
                var component = damageReport.victimBody;
                var networkUser = component.master.playerCharacterMasterController.networkUser;

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

                if (component.master.money <= 0) return;

                // Pick a random quip to add a little humor
                var rand = new Xoroshiro128Plus(Run.instance.stageRng.nextUlong);
                int index = rand.RangeInt(0, Quips.Length);

                // Get players alive and gold count
                var aliveLists = AliveList(component.master);
                uint money = component.master.money;
                uint count = Convert.ToUInt32(aliveLists.Count);

                // Return if there is no more alive players
                if (count < 1) return;

                // Zero the victim's gold and distribute it
                uint split = money / count;
                component.master.money = 0;
                foreach (var player in aliveLists)
                {
                    player.money += (uint)(split * goldMultiplier);
                }

                // Broadcast drop message
                if (quipsEnabled)
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "DGOD_DEATH_MESSAGE",
                        paramTokens = new [] { networkUser.userName, split.ToString(), Language.GetString(Quips[index]) }
                    });
                }
                else
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "DGOD_DEATH_MESSAGE_WITHOUT_QUIP",
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
            var players = new List<CharacterMaster>();

            foreach (var player in PlayerCharacterMasterController.instances)
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
