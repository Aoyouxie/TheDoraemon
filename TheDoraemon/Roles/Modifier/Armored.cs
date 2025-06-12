namespace TheDoraemon.Roles.Modifier
{
    public static class Armored
    {
        public static PlayerControl armored;

        public static bool isBrokenArmor = false;
        public static void clearAndReload()
        {
            armored = null;
            isBrokenArmor = false;
        }
    }
}
    