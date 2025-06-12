using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Modules;



namespace TheDoraemon.Roles.Impostors
{
    public static class Assassin
    {
        public static PlayerControl assassin;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl assassinMarked;
        public static PlayerControl currentTarget;
        public static float cooldown = 30f;
        public static float traceTime = 1f;
        public static bool knowsTargetLocation = false;
        private static Sprite markButtonSprite;
        private static Sprite killButtonSprite;
        public static Arrow arrow = new(Color.black);

        public static AchievementToken<(bool markKill, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (assassin == null || PlayerControl.LocalPlayer != assassin) return;
            acTokenChallenge ??= new("assassin.challenge", (false, false), (val, _) => val.cleared);
        }

        public static Sprite getMarkButtonSprite()
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.AssassinMarkButton.png", 115f);
            return markButtonSprite;
        }

        public static Sprite getKillButtonSprite()
        {
            if (killButtonSprite) return killButtonSprite;
            killButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.AssassinAssassinateButton.png", 115f);
            return killButtonSprite;
        }

        public static void clearAndReload()
        {
            assassin = null;
            currentTarget = assassinMarked = null;
            cooldown = CustomOptionHolder.assassinCooldown.getFloat();
            knowsTargetLocation = CustomOptionHolder.assassinKnowsTargetLocation.getBool();
            traceTime = CustomOptionHolder.assassinTraceTime.getFloat();
            //invisibleDuration = CustomOptionHolder.assassinInvisibleDuration.getFloat();
            //invisibleTimer = 0f;
            //isInvisble = false;
            if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.black);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
            acTokenChallenge = null;
        }
    }
}
    