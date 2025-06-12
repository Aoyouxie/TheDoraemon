using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Utilities;
using Hazel;
namespace TheDoraemon.Roles.Impostors
{
    public static class LastImpostor
    {
        public static PlayerControl lastImpostor;
        public static bool isEnable;
        public static int killCounter;
        public static int maxKillCounter;
        public static bool isOriginalGuesser;
        public static int numShots;
        public static bool hasMultipleShots;

        public static void promoteToLastImpostor()
        {
            if (!isEnable || !HandleGuesser.isGuesserGm) return;

            var impList = new List<PlayerControl>();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.Data.Role.IsImpostor && !p.Data.IsDead && !p.Data.Disconnected) impList.Add(p);
            }
            if (impList.Count == 1)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ImpostorPromotesToLastImpostor, SendOption.Reliable, -1);
                writer.Write(impList[0].PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.impostorPromotesToLastImpostor(impList[0].PlayerId);
            }
        }

        public static bool isCounterMax()
        {
            if (maxKillCounter <= killCounter) return true;
            return false;
        }

        public static void clearAndReload()
        {
            lastImpostor = null;
            isEnable = CustomOptionHolder.guesserGamemodeEnableLastImpostor.getBool();
            killCounter = 0;
            maxKillCounter = Mathf.RoundToInt(CustomOptionHolder.guesserGamemodeLastImpostorNumKills.getFloat());
            isOriginalGuesser = false;
            numShots = Mathf.RoundToInt(CustomOptionHolder.guesserGamemodeLastImpostorNumShots.getFloat());
            hasMultipleShots = CustomOptionHolder.guesserGamemodeLastImpostorHasMultipleShots.getBool();
        }
    }
}
    