using UnityEngine;
namespace TheDoraemon.Roles.Crewmates
{
    public static class Veteran
    {
        public static PlayerControl veteran;
        public static Color color = new Color32(255, 77, 0, byte.MaxValue);

        public static float alertDuration = 3f;
        public static float cooldown = 30f;

        public static int remainingAlerts = 5;

        public static bool alertActive = false;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.AlertButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            veteran = null;
            alertActive = false;
            alertDuration = CustomOptionHolder.veteranAlertDuration.getFloat();
            cooldown = CustomOptionHolder.veteranCooldown.getFloat();
            remainingAlerts = Mathf.RoundToInt(CustomOptionHolder.veteranAlertNumber.getFloat());
        }
    }
}
    