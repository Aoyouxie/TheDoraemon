using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using Hazel;
using Reactor.Utilities.Extensions;
using TheDoraemon.Modules;

namespace TheDoraemon.Roles.Crewmates
{
    public static class Tracker {
        public static PlayerControl tracker;
        public static Color color = new Color32(100, 58, 220, byte.MaxValue);
        public static List<Arrow> localArrows = new();

        public static float updateIntervall = 5f;
        public static bool resetTargetAfterMeeting = false;
        public static bool canTrackCorpses = false;
        public static float corpsesTrackingCooldown = 30f;
        public static float corpsesTrackingDuration = 5f;
        public static float corpsesTrackingTimer = 0f;
        public static int trackingMode = 0;
        public static List<Vector3> deadBodyPositions = new();

        public static PlayerControl currentTarget;
        public static PlayerControl tracked;
        public static bool usedTracker = false;
        public static float timeUntilUpdate = 0f;
        public static Arrow arrow = new(Color.blue);

        public static GameObject DangerMeterParent;
        public static DangerMeter Meter;

        public static AchievementToken<(bool inVent, float ventTime, bool cleared)> acTokenChallenge = null;

        public static void unlockAch(float ventTime)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockTrackerAcChallenge, SendOption.Reliable, -1);
            writer.Write(ventTime);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.unlockTrackerAcChallenge(ventTime);
        }

        public static void onAchievementActivate()
        {
            if (tracker == null || PlayerControl.LocalPlayer != tracker) return;
            acTokenChallenge ??= new("tracker.challenge", (false, 0f, false), (val, _) => val.cleared);
        }

        private static Sprite trackCorpsesButtonSprite;
        public static Sprite getTrackCorpsesButtonSprite()
        {
            if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
            trackCorpsesButtonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.PathfindButton.png", 115f);
            return trackCorpsesButtonSprite;
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.TrackerButton.png", 115f);
            return buttonSprite;
        }

        public static void resetTracked() {
            currentTarget = tracked = null;
            timeUntilUpdate = 0f;
            usedTracker = false;
            if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
            if (DangerMeterParent)
            {
                Meter.gameObject.Destroy();
                DangerMeterParent.Destroy();
            }
        }

        public static void clearAndReload() {
            tracker = null;
            resetTracked();
            timeUntilUpdate = 0f;
            updateIntervall = CustomOptionHolder.trackerUpdateIntervall.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.getBool();
            if (localArrows != null) {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            deadBodyPositions = new List<Vector3>();
            corpsesTrackingTimer = 0f;
            corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.getFloat();
            corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.getFloat();
            canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.getBool();
            trackingMode = CustomOptionHolder.trackerTrackingMethod.getSelection();
            acTokenChallenge = null;
        }
    }
}
    