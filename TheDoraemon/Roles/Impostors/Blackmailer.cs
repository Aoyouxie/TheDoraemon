using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class Blackmailer
    {
        public static PlayerControl blackmailer;
        public static Color color = Palette.ImpostorRed;
        public static Color blackmailedColor = Palette.White;

        public static bool alreadyShook = false;
        public static PlayerControl blackmailed;
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;

        public static AchievementToken<(List<byte> witness, bool cleared)> acTokenChallenge;
        private static Sprite blackmailButtonSprite;
        private static Sprite overlaySprite;

        public static void onAchievementActivate()
        {
            if (blackmailer == null || PlayerControl.LocalPlayer != blackmailer) return;
            acTokenChallenge ??= new("blackmailer.challenge", (new List<byte>(), false), (val, _) => val.cleared);
        }

        public static Sprite getBlackmailOverlaySprite()
        {
            if (overlaySprite) return overlaySprite;
            overlaySprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.BlackmailerOverlay.png", 100f);
            return overlaySprite;
        }

        public static Sprite getBlackmailButtonSprite()
        {
            if (blackmailButtonSprite) return blackmailButtonSprite;
            blackmailButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.BlackmailerBlackmailButton.png", 115f);
            return blackmailButtonSprite;
        }

        public static void clearAndReload()
        {
            blackmailer = null;
            currentTarget = null;
            blackmailed = null;
            alreadyShook = false;
            cooldown = CustomOptionHolder.blackmailerCooldown.getFloat();
        }
    }
}
    