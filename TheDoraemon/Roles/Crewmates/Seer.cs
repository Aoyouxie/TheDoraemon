using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Modules;

namespace TheDoraemon.Roles.Crewmates
{
    public static class Seer
    {
        public static PlayerControl seer;
        public static Color color = new Color32(97, 178, 108, byte.MaxValue);
        public static List<Vector3> deadBodyPositions = new();

        public static float soulDuration = 15f;
        public static bool limitSoulDuration = false;
        public static int mode = 0;

        public static AchievementToken<(byte flash, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (seer == null || PlayerControl.LocalPlayer != seer) return;
            acTokenChallenge ??= new("seer.challenge", (0, false), (val, _) => val.flash >= 5 || val.cleared);
        }

        private static Sprite soulSprite;
        public static Sprite getSoulSprite()
        {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.Soul.png", 500f);
            return soulSprite;
        }

        public static void clearAndReload()
        {
            seer = null;
            deadBodyPositions = new List<Vector3>();
            limitSoulDuration = CustomOptionHolder.seerLimitSoulDuration.getBool();
            soulDuration = CustomOptionHolder.seerSoulDuration.getFloat();
            mode = CustomOptionHolder.seerMode.getSelection();
            acTokenChallenge = null;
        }
    }
}
    