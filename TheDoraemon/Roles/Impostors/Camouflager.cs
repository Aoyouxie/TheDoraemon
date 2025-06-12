using UnityEngine;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class Camouflager {
        public static PlayerControl camouflager;
        public static Color color = Palette.ImpostorRed;
    
        public static float cooldown = 30f;
        public static float duration = 10f;
        public static float camouflageTimer = 0f;
        public static AchievementToken<(int kills, bool cleared)> acTokenChallenge = null;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.CamoButton.png", 115f);
            return buttonSprite;
        }

        public static void onAchievementActivate()
        {
            if (camouflager == null || PlayerControl.LocalPlayer != camouflager) return;
            acTokenChallenge ??= new("camouflager.challenge", (0, false), (val, _) => val.cleared);
        }

        public static void resetCamouflage() {
            camouflageTimer = 0f;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                /*if ((p == Ninja.ninja && Ninja.stealthed) || (p == Sprinter.sprinter && Sprinter.sprinting))
                    continue;*/
                p.setDefaultLook();
            }
            if (PlayerControl.LocalPlayer == camouflager)
                acTokenChallenge.Value.kills = 0;
        }

        public static void clearAndReload() {
            resetCamouflage();
            camouflager = null;
            camouflageTimer = 0f;
            cooldown = CustomOptionHolder.camouflagerCooldown.getFloat();
            duration = CustomOptionHolder.camouflagerDuration.getFloat();
            acTokenChallenge = null;
        }
    }
}
    