using System;
using UnityEngine;



namespace TheDoraemon.Roles.Neutrals
{
    public static class Cupid
    {
        public static PlayerControl cupid;
        public static Color color = new Color32(246, 152, 150, byte.MaxValue);

        public static PlayerControl lovers1;
        public static PlayerControl lovers2;
        public static PlayerControl shielded;
        public static PlayerControl currentTarget;
        public static PlayerControl shieldTarget;
        public static DateTime startTime = DateTime.UtcNow;
        public static bool isShieldOn = false;
        public static int timeLeft;
        public static float timeLimit;

        private static Sprite arrowSprite;
        public static Sprite getArrowSprite()
        {
            if (arrowSprite) return arrowSprite;
            arrowSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.CupidButton.png", 115f);
            return arrowSprite;
        }

        public static bool isCupidLover(this PlayerControl p) => p == lovers1 && lovers2 != null || p == lovers2 && lovers1 != null;
        public static PlayerControl getCupidLover(this PlayerControl p)
        {
            if (p == null) return null;
            if (p == lovers1) return lovers2;
            if (p == lovers2) return lovers1;
            return null;
        }

        public static bool existing() => lovers1 != null && lovers2 != null && !lovers1.Data.Disconnected && !lovers2.Data.Disconnected;

        public static bool existingAndAlive() => existing() && !lovers1.Data.IsDead && !lovers2.Data.IsDead;

        public static bool existingWithKiller()
        {
            return existing() && (Helpers.isKiller(lovers1) || Helpers.isKiller(lovers2)) && !(Thief.thief != null && (lovers1 == Thief.thief
                || lovers2 == Thief.thief));
        }

        public static bool hasAliveKillingCupidLover(this PlayerControl player)
        {
            if (!existingAndAlive() || !existingWithKiller())
                return false;
            return player != null && (player == lovers1 || player == lovers2);
        }

        public static void breakLovers(PlayerControl lover)
        {
            if (Lovers.lover1 != null && lover == Lovers.lover1 || Lovers.lover2 != null && lover == Lovers.lover2)
            {
                PlayerControl otherLover = lover.getPartner();
                if (otherLover != null && !otherLover.Data.IsDead)
                {
                    Lovers.clearAndReload();
                    otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                    GameHistory.overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoveStolen);
                }
            }
        }

        public static void clearAndReload(bool resetLovers = true)
        {
            cupid = null;
            if (resetLovers)
            {
                lovers1 = null;
                lovers2 = null;
            }
            shielded = null;
            currentTarget = null;
            shieldTarget = null;
            startTime = DateTime.UtcNow;
            timeLimit = CustomOptionHolder.cupidTimeLimit.getFloat() + 10f;
            timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
            isShieldOn = CustomOptionHolder.cupidShield.getBool();
        }
    }
}
    