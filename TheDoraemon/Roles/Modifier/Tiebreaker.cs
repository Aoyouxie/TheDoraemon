namespace TheDoraemon.Roles.Modifier
{
    public static class Tiebreaker
    {
        public static PlayerControl tiebreaker;

        public static bool isTiebreak = false;

        public static void clearAndReload()
        {
            tiebreaker = null;
            isTiebreak = false;
        }
    }
}
    