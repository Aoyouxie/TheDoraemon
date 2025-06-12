using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace TheDoraemon.Roles.Modifier
{
    public static class Madmate
    {
        public static Color color = Palette.ImpostorRed;
        public static List<PlayerControl> madmate = new();
        public static bool hasTasks;
        public static bool canDieToSheriff;
        public static bool canVent;
        public static bool hasImpostorVision;
        public static bool canFixComm;
        public static bool canSabotage;
        public static int commonTasks;
        public static int shortTasks;
        public static int longTasks;
        public static RoleId fixedRole;

        public static string fullName { get { return ModTranslation.getString("madmate"); } }
        public static string prefix { get { return ModTranslation.getString("madmatePrefix"); } }

        public static List<RoleId> validRoles = new()
        {
            RoleId.Jester,
            RoleId.Shifter,
            RoleId.Mayor,
            RoleId.Engineer,
            RoleId.TimespaceManager,
            RoleId.Lighter,
            RoleId.Detective,
            RoleId.TimeMaster,
            RoleId.Medic,
            RoleId.Swapper,
            RoleId.Seer,
            RoleId.Hacker,
            RoleId.Tracker,
            RoleId.SecurityGuard,
            RoleId.Bait,
            RoleId.Medium,
            RoleId.NiceGuesser,
            RoleId.NiceWatcher,
            RoleId.Busker,
            RoleId.Yasuna,
            RoleId.Noisemaker,
            RoleId.Archaeologist
        };

        public static bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = commonTasks + longTasks + shortTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter == totalTasks;
        }

        public static void clearAndReload()
        {
            hasTasks = CustomOptionHolder.madmateAbility.getBool();
            madmate = new List<PlayerControl>();
            canDieToSheriff = CustomOptionHolder.madmateCanDieToSheriff.getBool();
            canVent = CustomOptionHolder.madmateCanEnterVents.getBool();
            hasImpostorVision = CustomOptionHolder.madmateHasImpostorVision.getBool();
            canFixComm = CustomOptionHolder.madmateCanFixComm.getBool();
            canSabotage = CustomOptionHolder.madmateCanSabotage.getBool();
            shortTasks = (int)CustomOptionHolder.madmateShortTasks.getFloat();
            commonTasks = (int)CustomOptionHolder.madmateCommonTasks.getFloat();
            longTasks = (int)CustomOptionHolder.madmateLongTasks.getFloat();
            fixedRole = TORMapOptions.gameMode == CustomGamemodes.Guesser ? validRoles.Where(x => x != RoleId.NiceGuesser).ToList()[
                CustomOptionHolder.madmateFixedRoleGuesserGamemode.getSelection()] :
                validRoles[CustomOptionHolder.madmateFixedRole.getSelection()];
        }
    }
}
    