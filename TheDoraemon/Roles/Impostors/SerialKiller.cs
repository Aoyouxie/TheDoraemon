using UnityEngine;



namespace TheDoraemon.Roles.Impostors
{
    public static class SerialKiller
    {
        public static PlayerControl serialKiller;
        public static Color color = Palette.ImpostorRed;

        public static float killCooldown = 15f;
        public static float suicideTimer = 40f;
        public static bool resetTimer = true;

        public static bool isCountDown = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.SuicideButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            serialKiller = null;
            killCooldown = CustomOptionHolder.serialKillerKillCooldown.getFloat();
            suicideTimer = Mathf.Max(CustomOptionHolder.serialKillerSuicideTimer.getFloat(), killCooldown + 2.5f);
            resetTimer = CustomOptionHolder.serialKillerResetTimer.getBool();
            isCountDown = false;
        }
    }
}
    