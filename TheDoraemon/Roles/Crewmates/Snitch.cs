using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
namespace TheDoraemon.Roles.Crewmates
{
    public static class Snitch {
        public static PlayerControl snitch;
        public static Color color = new Color32(184, 251, 79, byte.MaxValue);

        public static List<Arrow> localArrows = new();
        public static int taskCountForReveal = 1;
        public static bool includeTeamEvil = false;
        public static bool teamEvilUseDifferentArrowColor = true;

        public static void clearAndReload() {
            if (localArrows != null)
            {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            localArrows = new List<Arrow>();
            taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.getFloat());
            includeTeamEvil = CustomOptionHolder.snitchIncludeTeamEvil.getBool();
            teamEvilUseDifferentArrowColor = CustomOptionHolder.snitchTeamEvilUseDifferentArrowColor.getBool();
            snitch = null;
        }
    }
}
    