using UnityEngine;
using Hazel;
using  TheDoraemon.Modules;
namespace TheDoraemon
{
        public static class Detective {
            public static PlayerControl detective;
            public static Color color = new Color32(45, 106, 165, byte.MaxValue);

            public static float footprintIntervall = 1f;
            public static float footprintDuration = 1f;
            public static bool anonymousFootprints = false;
            public static float reportNameDuration = 0f;
            public static float reportColorDuration = 20f;
            public static float timer = 6.2f;

            public static AchievementToken<bool> acTokenCommon = null;
            public static AchievementToken<(bool reported, byte votedFor, byte killerId, bool cleared)> acTokenChallenge = null;

            public static void onAchievementActivate()
            {
                if (detective == null || PlayerControl.LocalPlayer != detective) return;
                acTokenChallenge ??= new("detective.challenge", (false, byte.MaxValue, byte.MaxValue, false), (val, _) => val.cleared);
                acTokenCommon ??= new("detective.common1", false, (val, _) => val);
            }

            public static void unlockAch(byte votedFor)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockDetectiveAcChallenge, SendOption.Reliable, -1);
                writer.Write(votedFor);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unlockDetectiveAcChallenge(votedFor);
            }

            public static void clearAndReload() {
                detective = null;
                anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.getBool();
                footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.getFloat();
                footprintDuration = CustomOptionHolder.detectiveFootprintDuration.getFloat();
                reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.getFloat();
                reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.getFloat();
                timer = 6.2f;
                acTokenCommon = null;
                acTokenChallenge = null;
            }
        }
    }

