using HarmonyLib;
using Hazel;
using System;
using UnityEngine;
//using static TheDoraemon.TheDoraemon;
//using TheDoraemon.Objects;
using System.Linq;
using System.Collections.Generic;
using TheDoraemon.Roles;
//using TheDoraemon.Utilities;
//using TheDoraemon.CustomGameModes;
//using TheDoraemon.Patches;
//using TheDoraemon.Modules;
//using TheDoraemon.MetaContext;
//using Reactor.Utilities;

namespace TheDoraemon
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    static class HudManagerStartPatch
    {
        private static bool initialized = false;
        public static CustomButton timeSpaceManagerKillButton;


        public static Dictionary<byte, List<CustomButton>> deputyHandcuffedButtons = null;
        public static PoolablePlayer targetDisplay;

        public static void setCustomButtonCooldowns()
        {
            if (!initialized)
            {
                try
                {
                    createButtonsPostfix(HudManager.Instance);
                }
                catch
                {
                    TheDoraemonPlugin.Logger.LogWarning("Button cooldowns not set, either the gamemode does not require them or there's something wrong.");
                    return;
                }
            }
            timeSpaceManagerKillButton.MaxTimer = TimeSpaceManager.cooldown;
            //实现警长击杀
            timeSpaceManagerKillButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murderAttemptResult = Helpers.checkMuderAttempt(TimeSpaceManager.timeSpaceManager, TimeSpaceManager.currentTarget);
                    if (murderAttemptResult == MurderAttemptResult.SuppressKill) return;

                    if (murderAttemptResult is MurderAttemptResult.PerformKill or MurderAttemptResult.ReverseKill)
                    {
                        byte targetId = 0;
                        if (((TimeSpaceManager.currentTarget.Data.Role.IsImpostor && (TimeSpaceManager.currentTarget != Mini.mini || Mini.isGrownUp())) ||
                            (TimeSpaceManager.spyCanDieToTimeSpaceManager && Spy.spy == TimeSpaceManager.currentTarget) ||
                            (TimeSpaceManager.canKillNeutrals && Helpers.isNeutral(TimeSpaceManager.currentTarget)) ||
                            Jackal.jackal == TimeSpaceManager.currentTarget || Sidekick.sidekick == TimeSpaceManager.currentTarget ||
                            (CreatedMadmate.createdMadmate == TimeSpaceManager.currentTarget && CreatedMadmate.canDieToTimeSpaceManager) ||
                            (Madmate.canDieToTimeSpaceManager && Madmate.madmate.Any(x => x.PlayerId == TimeSpaceManager.currentTarget.PlayerId))) &&
                            !Madmate.madmate.Any(y => y.PlayerId == TimeSpaceManager.timeSpaceManager.PlayerId))
                        {
                            _ = new StaticAchievementToken("timeSpaceManager.common1");
                            targetId = TimeSpaceManager.currentTarget.PlayerId;
                        }
                        else
                        {
                            _ = new StaticAchievementToken("timeSpaceManager.another1");
                            targetId = PlayerControl.LocalPlayer.PlayerId;
                        }

                        MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                        killWriter.Write(TimeSpaceManager.timeSpaceManager.Data.PlayerId);
                        killWriter.Write(targetId);
                        killWriter.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                        RPCProcedure.uncheckedMurderPlayer(TimeSpaceManager.timeSpaceManager.Data.PlayerId, targetId, Byte.MaxValue);
                    }

                    timeSpaceManagerKillButton.Timer = timeSpaceManagerKillButton.MaxTimer;
                    TimeSpaceManager.currentTarget = null;
                },
                () => { return TimeSpaceManager.timeSpaceManager != null && TimeSpaceManager.timeSpaceManager == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return TimeSpaceManager.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { timeSpaceManagerKillButton.Timer = timeSpaceManagerKillButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.upperRowRight,
                __instance,
                KeyCode.Q
            );
        }
    }
}