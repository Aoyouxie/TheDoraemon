using UnityEngine;
using TheDoraemon.Modules;

namespace TheDoraemon.Roles.Crewmates
{
        public static class Engineer {
            public static PlayerControl engineer;
            public static Color color = new Color32(0, 40, 245, byte.MaxValue);
            private static Sprite buttonSprite;

            public static int remainingFixes = 1;           
            public static bool highlightForImpostors = true;
            public static bool highlightForTeamJackal = true;

            public static AchievementToken<(bool inVent, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (engineer == null || PlayerControl.LocalPlayer != engineer) return;
                acTokenChallenge ??= new("engineer.another1", (false, false), (val, _) => val.cleared);
            }

            public static Sprite getButtonSprite() {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.RepairButton.png", 115f);
                return buttonSprite;
            }

            public static void clearAndReload() {
                engineer = null;
                remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.getFloat());
                highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.getBool();
                highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.getBool();
                acTokenChallenge = null;
            }
        }
}

    