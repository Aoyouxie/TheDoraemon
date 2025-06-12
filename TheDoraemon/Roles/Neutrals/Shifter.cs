using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Modules;



namespace TheDoraemon.Roles.Neutrals
{
    public static class Shifter
    {
        public static PlayerControl shifter;
        public static List<int> pastShifters = new();
        public static Color color = new Color32(102, 102, 102, byte.MaxValue);

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;
        public static bool shiftModifiers = false;

        public static bool isNeutral = false;
        public static bool shiftPastShifters = false;
        public static bool shiftsMedicShield = false;

        public static AchievementToken<(byte shiftId, byte oldShifterId, bool cleared)> niceShifterAcTokenChallenge = null;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static void niceShifterOnAchievementActivate()
        {
            niceShifterAcTokenChallenge = null;
            if (shifter != null && PlayerControl.LocalPlayer == shifter && !isNeutral)
            {
                niceShifterAcTokenChallenge ??= new("niceShifter.challenge", (byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
            }
        }

        public static void clearAndReload()
        {
            shifter = null;
            pastShifters = new List<int>();
            currentTarget = null;
            futureShift = null;
            shiftModifiers = CustomOptionHolder.shifterShiftsModifiers.getBool();
            shiftPastShifters = CustomOptionHolder.shifterPastShifters.getBool();
            shiftsMedicShield = CustomOptionHolder.shifterShiftsMedicShield.getBool();
            isNeutral = false;
        }
    }
}
    