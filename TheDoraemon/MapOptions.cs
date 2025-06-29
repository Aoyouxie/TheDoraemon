using System.Collections.Generic;
using UnityEngine;

namespace TheDoraemon{
    static class TORMapOptions {
        // Set values
        public static int maxNumberOfMeetings = 10;
        public static bool blockSkippingInEmergencyMeetings = false;
        public static bool noVoteIsSelfVote = false;
        public static bool hidePlayerNames = false;
        public static bool ghostsSeeRoles = true;
        public static bool ghostsSeeModifier = true;
        public static bool ghostsSeeInformation = true;
        public static bool ghostsSeeVotes = true;
        public static bool showRoleSummary = true;
        public static bool allowParallelMedBayScans = false;
        public static bool showLighterDarker = true;
        public static bool toggleCursor = true;
        public static bool enableSoundEffects = true;
        public static bool enableHorseMode = false;
        public static bool shieldFirstKill = false;
        public static bool ShowChatNotifications = true;
        public static bool ShowVentsOnMap = true;
        public static bool showExtraInfo = true;
        public static CustomGamemodes gameMode = CustomGamemodes.Classic;

        // Updating values
        public static int meetingsCount = 0;
        public static List<SurvCamera> camerasToAdd = new();
        public static List<Vent> ventsToSeal = new();
        public static Dictionary<byte, PoolablePlayer> playerIcons = new();
        public static string firstKillName;
        public static PlayerControl firstKillPlayer;

        public static void clearAndReloadMapOptions() {
            meetingsCount = 0;
            camerasToAdd = new List<SurvCamera>();
            ventsToSeal = new List<Vent>();
            playerIcons = new Dictionary<byte, PoolablePlayer>();

            maxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.maxNumberOfMeetings.getSelection());
            blockSkippingInEmergencyMeetings = CustomOptionHolder.blockSkippingInEmergencyMeetings.getBool();
            noVoteIsSelfVote = CustomOptionHolder.noVoteIsSelfVote.getBool();
            hidePlayerNames = CustomOptionHolder.hidePlayerNames.getBool();
            allowParallelMedBayScans = CustomOptionHolder.allowParallelMedBayScans.getBool();
            shieldFirstKill = CustomOptionHolder.shieldFirstKill.getBool();
            firstKillPlayer = null;
        }

        public static void reloadPluginOptions() {
            ghostsSeeRoles = TheDoraemonPlugin.GhostsSeeRoles.Value;
            ghostsSeeModifier = TheDoraemonPlugin.GhostsSeeModifier.Value;
            ghostsSeeInformation = TheDoraemonPlugin.GhostsSeeInformation.Value;
            ghostsSeeVotes = TheDoraemonPlugin.GhostsSeeVotes.Value;
            showRoleSummary = TheDoraemonPlugin.ShowRoleSummary.Value;
            showLighterDarker = TheDoraemonPlugin.ShowLighterDarker.Value;
            toggleCursor = TheDoraemonPlugin.ToggleCursor.Value;
            enableSoundEffects = TheDoraemonPlugin.EnableSoundEffects.Value;
            enableHorseMode = TheDoraemonPlugin.EnableHorseMode.Value;
            ShowChatNotifications = TheDoraemonPlugin.ShowChatNotifications.Value;
            ShowVentsOnMap = TheDoraemonPlugin.ShowVentsOnMap.Value;
            showExtraInfo = TheDoraemonPlugin.ShowExtraInfo.Value;
            //Patches.ShouldAlwaysHorseAround.isHorseMode = TheDoraemonPlugin.EnableHorseMode.Value;
        }

        public static void resetPoolables() {
            foreach (PoolablePlayer p in playerIcons.Values) {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
        }
    }
}
