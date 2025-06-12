using System;
using UnityEngine;
using TheDoraemon.Objects;
using Hazel;
using TheDoraemon.Modules;



namespace TheDoraemon.Roles.Impostors
{
    public static class Trapper
    {
        public static PlayerControl trapper;
        public static Color color = Palette.ImpostorRed;

        public static float minDistance = 0f;
        public static float maxDistance;
        public static int numTrap;
        public static float extensionTime;
        public static float killTimer;
        public static float cooldown;
        public static float trapRange;
        public static float penaltyTime;
        public static float bonusTime;
        public static bool isTrapKill = false;
        public static bool meetingFlag;

        public static Sprite trapButtonSprite;
        public static DateTime placedTime;

        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (trapper == null || PlayerControl.LocalPlayer != trapper) return;
            acTokenChallenge ??= new("trapper.challenge", 0, (val, _) => val >= 3);
        }

        public static Sprite getTrapButtonSprite()
        {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.TrapperButton.png", 115f);
            return trapButtonSprite;
        }

        public static void setTrap()
        {
            var pos = PlayerControl.LocalPlayer.transform.position;
            byte[] buff = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceTrap, SendOption.Reliable);
            writer.WriteBytesAndSize(buff);
            writer.EndMessage();
            RPCProcedure.placeTrap(buff);
            placedTime = DateTime.UtcNow;
        }

        public static void clearAndReload()
        {
            trapper = null;
            numTrap = (int)CustomOptionHolder.trapperNumTrap.getFloat();
            extensionTime = CustomOptionHolder.trapperExtensionTime.getFloat();
            killTimer = CustomOptionHolder.trapperKillTimer.getFloat();
            cooldown = CustomOptionHolder.trapperCooldown.getFloat();
            trapRange = CustomOptionHolder.trapperTrapRange.getFloat();
            penaltyTime = CustomOptionHolder.trapperPenaltyTime.getFloat();
            bonusTime = CustomOptionHolder.trapperBonusTime.getFloat();
            maxDistance = CustomOptionHolder.trapperMaxDistance.getFloat();
            meetingFlag = false;
            Trap.clearAllTraps();
            acTokenChallenge = null;
        }
    }
}
    