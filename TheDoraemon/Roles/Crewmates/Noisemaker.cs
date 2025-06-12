using UnityEngine;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Crewmates
{
    public static class Noisemaker
    {
        public static Color32 color = new(160, 131, 187, byte.MaxValue);
        public static PlayerControl noisemaker;
        public static PlayerControl currentTarget;
        public static PlayerControl target;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (noisemaker == null || PlayerControl.LocalPlayer != noisemaker) return;
            acTokenChallenge ??= new("noisemaker.challenge", 0, (val, _) => val >= 3);
        }

        public enum SoundTarget
        {
            Noisemaker,
            Crewmates,
            Everyone
        }

        public static float cooldown;
        public static float duration;
        public static int numSound;
        public static SoundTarget soundTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.NoisemakerButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            noisemaker = null;
            cooldown = CustomOptionHolder.noisemakerCooldown.getFloat();
            duration = CustomOptionHolder.noisemakerSoundDuration.getFloat();
            numSound = Mathf.RoundToInt(CustomOptionHolder.noisemakerSoundNumber.getFloat());
            soundTarget = (SoundTarget)CustomOptionHolder.noisemakerSoundTarget.getSelection();
            acTokenChallenge = null;
        }
    }
}
    