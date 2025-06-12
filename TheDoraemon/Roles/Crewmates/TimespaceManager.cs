using TheDoraemon.Modules;
using UnityEngine;
using HarmonyLib;

namespace TheDoraemon.Roles.Crewmates
{
    [HarmonyPatch]
   
            public static class TimespaceManager
          {
            public static PlayerControl timespaceManager;
            public static Color color = new Color32(248, 205, 70, byte.MaxValue);

            public static float cooldown = 30f;
            public static bool canKillNeutrals = false;
            //public static bool spyCanDieToSheriff = false;

            public static PlayerControl currentTarget;

            public static PlayerControl formerDeputy;  // Needed for keeping handcuffs + shifting
            public static PlayerControl formerSheriff;  // When deputy gets promoted...

            public static AchievementToken<(bool isTriggeredFalse, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (timespaceManager == null || PlayerControl.LocalPlayer != timespaceManager) return;
                acTokenChallenge ??= new("timespaceManager.challenge", (true, true), (val, _) => val.cleared && !val.isTriggeredFalse);
            }

            public static void replaceCurrentSheriff(PlayerControl deputy)
            {
                if (!formerSheriff) formerSheriff = timespaceManager;
                timespaceManager = deputy;
                currentTarget = null;
                cooldown = CustomOptionHolder.timespaceManagerCooldown.getFloat();
            }

            public static void clearAndReload()
            {
                timespaceManager = null;
                currentTarget = null;
                formerDeputy = null;
                formerSheriff = null;
                cooldown = CustomOptionHolder.timespaceManagerCooldown.getFloat();
                canKillNeutrals = CustomOptionHolder.timespaceManagerCanKillNeutrals.getBool();
                //spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.getBool();
                acTokenChallenge = null;
            }
        }
}
