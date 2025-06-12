using UnityEngine;
namespace TheDoraemon
{
    public static class TaskMaster
    {
        public static PlayerControl taskMaster = null;
        public static bool becomeATaskMasterWhenCompleteAllTasks = false;
        public static Color color = new Color32(225, 86, 75, byte.MaxValue);
        public static bool isTaskComplete = false;
        public static byte clearExTasks = 0;
        public static byte allExTasks = 0;
        public static byte oldTaskMasterPlayerId = byte.MaxValue;
        public static bool triggerTaskMasterWin = false;

        public static void clearAndReload()
        {
            taskMaster = null;
            becomeATaskMasterWhenCompleteAllTasks = CustomOptionHolder.taskMasterBecomeATaskMasterWhenCompleteAllTasks.getBool();
            isTaskComplete = false;
            clearExTasks = 0;
            allExTasks = 0;
            oldTaskMasterPlayerId = byte.MaxValue;
            triggerTaskMasterWin = false;
        }

        public static bool isTaskMaster(byte playerId)
        {
            return taskMaster != null && taskMaster.PlayerId == playerId;
        }
    }
}
    