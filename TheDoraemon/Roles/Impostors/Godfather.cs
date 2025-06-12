using UnityEngine;
namespace TheDoraemon.Roles.Impostors
{
    
        public static class Godfather {
            public static PlayerControl godfather;
            public static Color color = Palette.ImpostorRed;

            public static void clearAndReload() {
                godfather = null;
            }
        }
}
    