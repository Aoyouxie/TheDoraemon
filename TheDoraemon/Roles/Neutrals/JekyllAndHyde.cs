using System.Linq;
using System;
using UnityEngine;
using TheDoraemon.Utilities;



namespace TheDoraemon.Roles.Neutrals
{
    public static class JekyllAndHyde
    {
        public static Color color = Color.grey;
        public static PlayerControl jekyllAndHyde;
        public static PlayerControl formerJekyllAndHyde;
        public static PlayerControl currentTarget;

        public enum Status
        {
            None,
            Jekyll,
            Hyde,
        }

        public static Status status;
        public static int counter = 0;
        public static int numberToWin = 3;
        public static float suicideTimer = 40f;
        public static bool reset = true;
        public static float cooldown = 18f;
        public static int numUsed;
        public static bool oddIsJekyll;
        public static bool triggerWin = false;
        public static int numCommonTasks;
        public static int numLongTasks;
        public static int numShortTasks;
        public static int numTasks;

        public static bool isOdd(int n)
        {
            return n % 2 == 1;
        }

        public static bool isJekyll()
        {
            if (status == Status.None)
            {
                var alive = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x =>
                {
                    return !x.Data.IsDead;
                });
                bool ret = oddIsJekyll ? isOdd(alive.Count()) : !isOdd(alive.Count());
                return ret;
            }
            return status == Status.Jekyll;
        }

        public static int getNumDrugs()
        {
            var p = jekyllAndHyde;
            int counter = p.Data.Tasks.ToArray().Where(t => t.Complete).Count();
            return (int)Math.Floor((float)counter / numTasks);
        }

        public static void clearAndReload()
        {
            jekyllAndHyde = null;
            formerJekyllAndHyde = null;
            currentTarget = null;
            status = Status.None;
            counter = 0;
            triggerWin = false;
            numUsed = 0;
            numTasks = (int)CustomOptionHolder.jekyllAndHydeNumTasks.getFloat();
            numCommonTasks = (int)CustomOptionHolder.jekyllAndHydeCommonTasks.getFloat();
            numShortTasks = (int)CustomOptionHolder.jekyllAndHydeShortTasks.getFloat();
            numLongTasks = (int)CustomOptionHolder.jekyllAndHydeLongTasks.getFloat();
            reset = CustomOptionHolder.jekyllAndHydeResetAfterMeeting.getBool();
            numberToWin = (int)CustomOptionHolder.jekyllAndHydeNumberToWin.getFloat();
            cooldown = CustomOptionHolder.jekyllAndHydeCooldown.getFloat();
            suicideTimer = CustomOptionHolder.jekyllAndHydeSuicideTimer.getFloat();
        }
    }
}
    