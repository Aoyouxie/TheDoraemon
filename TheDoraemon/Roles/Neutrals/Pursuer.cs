using System.Collections.Generic;
using UnityEngine;



namespace TheDoraemon.Roles.Neutrals
{
    public static class Pursuer {
        public static PlayerControl pursuer;
        public static PlayerControl target;
        public static List<PlayerControl> blankedList = new();
        public static int blanks = 0;
        public static Sprite blank;
        public static bool notAckedExiled = false;

        public static float cooldown = 30f;
        public static int blanksNumber = 5;

        public static Sprite getTargetSprite() {
            if (blank) return blank;
            blank = Helpers.loadSpriteFromResources("TheDoraemon.Resources.PursuerButton.png", 115f);
            return blank;
        }

        public static void clearAndReload() {
            pursuer = null;
            target = null;
            blankedList = new List<PlayerControl>();
            blanks = 0;
            notAckedExiled = false;
        }
    }
}
    