using UnityEngine;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Crewmates
{
    public static class Bait
    {
        public static PlayerControl bait;
        public static Color color = new Color32(0, 247, 255, byte.MaxValue);

        public static bool highlightAllVents = false;
        public static float reportDelay = 0f;
        public static bool showKillFlash = true;
        public static bool canBeGuessed = true;

        public static bool reported = false;

        public static AchievementToken<(byte killerId, bool cleared)> acTokenChallenge = null;

        public static void onAchievementActivate()
        {
            if (bait == null || PlayerControl.LocalPlayer != bait) return;
            acTokenChallenge ??= new("bait.challenge", (byte.MaxValue, false), (val, _) => val.cleared);
        }

        public static void clearAndReload()
        {
            bait = null;
            reported = false;
            highlightAllVents = CustomOptionHolder.baitHighlightAllVents.getBool();
            reportDelay = CustomOptionHolder.baitReportDelay.getFloat();
            showKillFlash = CustomOptionHolder.baitShowKillFlash.getBool();
            canBeGuessed = CustomOptionHolder.baitCanBeGuessed.getBool();
            acTokenChallenge = null;
        }
    }

}
    