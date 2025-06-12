using UnityEngine;



namespace TheDoraemon.Roles.Impostors
{
    public static class NekoKabocha
    {
        public static PlayerControl nekoKabocha;
        public static Color color = Palette.ImpostorRed;

        public static bool revengeCrew = true;
        public static bool revengeImpostor = true;
        public static bool revengeNeutral = true;
        public static bool revengeExile = false;

        public static PlayerControl meetingKiller = null;
        public static PlayerControl otherKiller;

        public static void clearAndReload()
        {
            nekoKabocha = null;
            meetingKiller = null;
            otherKiller = null;
            revengeCrew = CustomOptionHolder.nekoKabochaRevengeCrew.getBool();
            revengeImpostor = CustomOptionHolder.nekoKabochaRevengeImpostor.getBool();
            revengeNeutral = CustomOptionHolder.nekoKabochaRevengeNeutral.getBool();
            revengeExile = CustomOptionHolder.nekoKabochaRevengeExile.getBool();
        }
    }
}
    