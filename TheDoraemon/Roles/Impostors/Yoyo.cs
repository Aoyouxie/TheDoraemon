using UnityEngine;



namespace TheDoraemon.Roles.Impostors
{
    public static class Yoyo
    {
        public static PlayerControl yoyo = null;
        public static Color color = Palette.ImpostorRed;

        public static float blinkDuration = 0;
        public static float markCooldown = 0;
        public static bool markStaysOverMeeting = false;
        public static float SilhouetteVisibility => silhouetteVisibility == 0 && (PlayerControl.LocalPlayer == yoyo || PlayerControl.LocalPlayer.Data.IsDead) ? 0.1f : silhouetteVisibility;
        public static float silhouetteVisibility = 0;

        public static Vector3? markedLocation = null;

        private static Sprite markButtonSprite;

        public static Sprite getMarkButtonSprite()
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.YoyoMarkButtonSprite.png", 115f);
            return markButtonSprite;
        }
        private static Sprite blinkButtonSprite;

        public static Sprite getBlinkButtonSprite()
        {
            if (blinkButtonSprite) return blinkButtonSprite;
            blinkButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.YoyoBlinkButtonSprite.png", 115f);
            return blinkButtonSprite;
        }

        public static void markLocation(Vector3 position)
        {
            markedLocation = position;
        }

        public static void clearAndReload()
        {
            yoyo = null;
            blinkDuration = CustomOptionHolder.yoyoBlinkDuration.getFloat();
            markCooldown = CustomOptionHolder.yoyoMarkCooldown.getFloat();
            markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.getBool();
            silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.getSelection() / 10f;
            markedLocation = null;
        }
    }
}
    