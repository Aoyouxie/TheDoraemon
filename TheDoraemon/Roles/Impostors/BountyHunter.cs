using System;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class BountyHunter {
        public static PlayerControl bountyHunter;
        public static Color color = Palette.ImpostorRed;

        public static Arrow arrow;
        public static float bountyDuration = 30f;
        public static bool showArrow = true;
        public static float bountyKillCooldown = 0f;
        public static float punishmentTime = 15f;
        public static float arrowUpdateIntervall = 10f;

        public static float arrowUpdateTimer = 0f;
        public static float bountyUpdateTimer = 0f;
        public static PlayerControl bounty;
        public static TMPro.TextMeshPro cooldownText;

        public static AchievementToken<(DateTime history, int kills, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (bountyHunter == null || PlayerControl.LocalPlayer != bountyHunter) return;
            acTokenChallenge ??= new("bountyHunter.challenge", (DateTime.UtcNow, 0, false), (val, _) => val.cleared);
        }

        public static void clearAllArrow() {
            if (PlayerControl.LocalPlayer != bountyHunter) return;
            if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            if (cooldownText) cooldownText.gameObject.SetActive(false);
            TORMapOptions.resetPoolables();
        }

        public static void clearAndReload() {
            arrow = new Arrow(color);
            bountyHunter = null;
            bounty = null;
            arrowUpdateTimer = 0f;
            bountyUpdateTimer = 0f;
            if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = null;
            if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
            cooldownText = null;
            TORMapOptions.resetPoolables();


            bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.getFloat();
            bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.getFloat();
            punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.getFloat();
            showArrow = CustomOptionHolder.bountyHunterShowArrow.getBool();
            arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.getFloat();
            acTokenChallenge = null;
        }
    }
}
    