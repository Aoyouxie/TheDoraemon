using UnityEngine;
using TheDoraemon.Modules;



namespace TheDoraemon.Roles.Neutrals
{
    public static class Guesser {
        public static PlayerControl niceGuesser;
        public static PlayerControl evilGuesser;
        public static Color color = new Color32(255, 255, 0, byte.MaxValue);

        public static int remainingShotsEvilGuesser = 2;
        public static int remainingShotsNiceGuesser = 2;

        public static AchievementToken<int> acTokenNiceGuesser;
        public static AchievementToken<int> acTokenEvilGuesser;

        public static void niceGuesserOnAchievementActivate()
        {
            if (niceGuesser == null || PlayerControl.LocalPlayer != niceGuesser) return;
            acTokenNiceGuesser ??= new("niceGuesser.challenge1", 0, (val, _) => val >= 3);
        }

        public static void evilGuesserOnAchievementActivate()
        {
            if (evilGuesser == null || PlayerControl.LocalPlayer != evilGuesser) return;
            acTokenEvilGuesser ??= new("evilGuesser.challenge1", 0, (val, _) => val >= 3);
        }

        public static bool isGuesser (byte playerId) {
            if (niceGuesser != null && niceGuesser.PlayerId == playerId || evilGuesser != null && evilGuesser.PlayerId == playerId) return true;
            return false;
        }

        public static void clear (byte playerId) {
            if (niceGuesser != null && niceGuesser.PlayerId == playerId) niceGuesser = null;
            else if (evilGuesser != null && evilGuesser.PlayerId == playerId) evilGuesser = null;
        }

        public static int remainingShots(byte playerId, bool shoot = false) {
            int remainingShots = remainingShotsEvilGuesser;
            if (niceGuesser != null && niceGuesser.PlayerId == playerId) {
                remainingShots = remainingShotsNiceGuesser;
                if (shoot) remainingShotsNiceGuesser = Mathf.Max(0, remainingShotsNiceGuesser - 1);
            } else if (shoot) {
                remainingShotsEvilGuesser = Mathf.Max(0, remainingShotsEvilGuesser - 1);
            }
            return remainingShots;
        }

        public static void clearAndReload() {
            niceGuesser = null;
            evilGuesser = null;
            remainingShotsEvilGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
            remainingShotsNiceGuesser = Mathf.RoundToInt(CustomOptionHolder.guesserNumberOfShots.getFloat());
            acTokenNiceGuesser = null;
            acTokenEvilGuesser = null;
        }
    }
}
    