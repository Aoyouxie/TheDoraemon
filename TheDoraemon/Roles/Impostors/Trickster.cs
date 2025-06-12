using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Modules;



namespace TheDoraemon.Roles.Impostors
{
    public static class Trickster {
        public static PlayerControl trickster;
        public static Color color = Palette.ImpostorRed;
        public static float placeBoxCooldown = 30f;
        public static float lightsOutCooldown = 30f;
        public static float lightsOutDuration = 10f;
        public static float lightsOutTimer = 0f;
        public static AchievementToken<(int kills, bool cleared)> acTokenChallenge;

        private static Sprite placeBoxButtonSprite;
        private static Sprite lightOutButtonSprite;
        private static Sprite tricksterVentButtonSprite;

        public static void onAchievementActivate()
        {
            if (trickster == null || PlayerControl.LocalPlayer != trickster) return;
            acTokenChallenge ??= new("trickster.challenge", (0, false), (val, _) => val.cleared || val.kills >= 2);
        }

        public static Sprite getPlaceBoxButtonSprite() {
            if (placeBoxButtonSprite) return placeBoxButtonSprite;
            placeBoxButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.PlaceJackInTheBoxButton.png", 115f);
            return placeBoxButtonSprite;
        }

        public static Sprite getLightsOutButtonSprite() {
            if (lightOutButtonSprite) return lightOutButtonSprite;
            lightOutButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.LightsOutButton.png", 115f);
            return lightOutButtonSprite;
        }

        public static Sprite getTricksterVentButtonSprite() {
            if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
            tricksterVentButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.TricksterVentButton.png", 115f);
            return tricksterVentButtonSprite;
        }

        public static void clearAndReload() {
            trickster = null;
            lightsOutTimer = 0f;
            placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.getFloat();
            lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.getFloat();
            lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.getFloat();
            JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
            acTokenChallenge = null;
        }

    }
}
    