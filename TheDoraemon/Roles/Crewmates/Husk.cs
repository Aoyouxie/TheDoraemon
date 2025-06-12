using System.Collections.Generic;
namespace TheDoraemon.Roles.Crewmates
{
    public static class Husk
    {
        public static List<PlayerControl> husk = new();
        public static void clearAndReload()
        {
            husk = new List<PlayerControl>();
        }
    }
}
    