using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
//using TheDoraemon.Objects;
//using TheDoraemon.Utilities;
//using TheDoraemon.CustomGameModes;
//using static TheDoraemonRoles;
//using AmongUs.Data;
//using Hazel;
//using TheDoraemon.Patches;
//using Reactor.Utilities.Extensions;
//using TheDoraemon.Modules;
//using AmongUs.GameOptions;
//using UnityEngine.Events;
//using UnityEngine.UI;
//using System.Collections;
//using Reactor.Utilities;
//using TheDoraemon.MetaContext;
//using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;
//using static UnityEngine.ParticleSystem.PlaybackState;
//using UnityEngine.UIElements;



namespace TheDoraemon.Roles
{
    public static class TheDoraemonRoles
    {
        [HarmonyPatch]
        public static class TheDoraemon
        {
            public static System.Random rnd = new((int)DateTime.Now.Ticks);

            public static void clearAndReloadRoles()
            {
                //以下职业使用了TheDoraemonGMIA代码
                TimeSpaceManager.clearAndReloadRoles();
            }
        }
        public static class TimeSpaceManager
        {
            public static PlayerControl timeSpaceManager;
            public static TimeSpaceManagerColor color = new Color32(248, 205, 70, byte.MaxValue);

            public static float cooldown = 30f;
            public static bool canKillNeutrals = false;
            public static bool spyCanDieToTimeSpaceManager = false;

            public static PlayerControl currentTarget;

            //public static PlayerControl formerDeputy;  // Needed for keeping handcuffs + shifting
            //public static PlayerControl formerTimeSpaceManager;  // When deputy gets promoted...

            /*
             public static AchievementToken<(bool isTriggeredFalse, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (timeSpaceManager == null || PlayerControl.LocalPlayer != timeSpaceManager) return;
                acTokenChallenge ??= new("timeSpaceManager.challenge", (true, true), (val, _) => val.cleared && !val.isTriggeredFalse);
            }
            */

            public static void replaceCurrentTimeSpaceManager(PlayerControl deputy)
            {
                if (!formerTimeSpaceManager) formerTimeSpaceManager = timeSpaceManager;
                timeSpaceManager = deputy;
                currentTarget = null;
                cooldown = CustomOptionHolder.timeSpaceManagerCooldown.getFloat();
            }

            public static void clearAndReload()
            {
                timeSpaceManager = null;
                currentTarget = null;
                formerDeputy = null;
                formerTimeSpaceManager = null;
                cooldown = CustomOptionHolder.timeSpaceManagerCooldown.getFloat();
                canKillNeutrals = CustomOptionHolder.timeSpaceManagerCanKillNeutrals.getBool();
                spyCanDieToTimeSpaceManager = CustomOptionHolder.spyCanDieToTimeSpaceManager.getBool();
                //acTokenChallenge = null;
            }

            internal static void clearAndReloadRoles()
            {
                throw new NotImplementedException();
            }
        }

    }

    public class TimeSpaceManagerColor
    {
    }
}
