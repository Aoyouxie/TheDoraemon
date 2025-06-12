namespace TheDoraemon.Roles.Crewmates
{
    public static class CreatedMadmate
    {
        public static PlayerControl createdMadmate;

        public static bool canEnterVents;
        public static bool hasImpostorVision;
        public static bool canSabotage;
        public static bool canFixComm;
        public static bool canDieToSheriff;
        public static bool hasTasks;
        public static int numTasks;

        public static bool tasksComplete(PlayerControl player)
        {
            if (!hasTasks) return false;

            int counter = 0;
            int totalTasks = numTasks;
            if (totalTasks == 0) return true;
            foreach (var task in player.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter >= totalTasks;
        }

        public static void clearAndReload()
        {
            createdMadmate = null;
            canEnterVents = CustomOptionHolder.createdMadmateCanEnterVents.getBool();
            canDieToSheriff = CustomOptionHolder.createdMadmateCanDieToSheriff.getBool();
            hasTasks = CustomOptionHolder.createdMadmateAbility.getBool();
            canSabotage = CustomOptionHolder.createdMadmateCanSabotage.getBool();
            canFixComm = CustomOptionHolder.createdMadmateCanFixComm.getBool();
            numTasks = (int)CustomOptionHolder.createdMadmateCommonTasks.getFloat();
        }
    }
}
    