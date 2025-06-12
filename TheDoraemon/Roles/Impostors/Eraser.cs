using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class Eraser {
        public static PlayerControl eraser;
        public static Color color = Palette.ImpostorRed;

        public static List<byte> alreadyErased = new();

        public static List<PlayerControl> futureErased = new();
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static bool canEraseAnyone = false;
        public static float cooldownIncrease = 10f;
        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (eraser == null || PlayerControl.LocalPlayer != eraser) return;
            acTokenChallenge ??= new("eraser.challenge", 0, (val, _) => val >= 3);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.EraserButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload() {
            eraser = null;
            futureErased = new List<PlayerControl>();
            currentTarget = null;
            cooldown = CustomOptionHolder.eraserCooldown.getFloat();
            canEraseAnyone = CustomOptionHolder.eraserCanEraseAnyone.getBool();
            cooldownIncrease = CustomOptionHolder.eraserCooldownIncrease.getFloat();
            alreadyErased = new List<byte>();
            acTokenChallenge = null;
        }
    }
}
    