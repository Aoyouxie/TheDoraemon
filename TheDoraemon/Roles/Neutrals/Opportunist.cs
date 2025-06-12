using UnityEngine;



namespace TheDoraemon.Roles.Neutrals
{
    public static class Opportunist
    {
        public static PlayerControl opportunist;
        public static Color color = new Color32(0, 255, 00, byte.MaxValue);

        public static void clearAndReload()
        {
            opportunist = null;
        }
    }
}
    