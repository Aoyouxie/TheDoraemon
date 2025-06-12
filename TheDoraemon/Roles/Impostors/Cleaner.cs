using UnityEngine;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class Cleaner {
        public static PlayerControl cleaner;
        public static Color color = Palette.ImpostorRed;

        public static float cooldown = 30f;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (cleaner == null || PlayerControl.LocalPlayer != cleaner) return;
            acTokenChallenge ??= new("cleaner.challenge", 0, (val, _) => val >= 3);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.CleanButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            cleaner = null;
            cooldown = CustomOptionHolder.cleanerCooldown.getFloat();
            acTokenChallenge = null;
        }
    }
}
    