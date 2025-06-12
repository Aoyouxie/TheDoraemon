using UnityEngine;
using Hazel;



namespace TheDoraemon.Roles.Neutrals
{
        public static class Jester {
            public static PlayerControl jester;
            public static Color color = new Color32(236, 98, 165, byte.MaxValue);

            public static bool triggerJesterWin = false;
            public static bool canCallEmergency = true;
            public static bool hasImpostorVision = false;
            public static bool canUseVents = false;

            public static void unlockAch()
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockJesterAcCommon, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unlockJesterAcCommon();
            }

            public static void clearAndReload() {
                jester = null;
                triggerJesterWin = false;
                canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.getBool();
                hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.getBool();
                canUseVents = CustomOptionHolder.jesterCanVent.getBool();
            }
        }
    }
    