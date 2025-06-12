using System.Collections.Generic;
using UnityEngine;



namespace TheDoraemon.Roles.Neutrals
{
    public static class Doomsayer
    {
        public static PlayerControl doomsayer;
        public static Color color = new(0f, 1f, 0.5f, 1f);

        public static int guessesToWin = 4;
        public static int counter = 0;
        public static bool hasMultipleGuesses = true;
        public static float cooldown = 30f;
        public static bool indicateGuesses = true;
        public static bool canObserve = false;
        public static bool triggerWin = false;
        public static int failedGuesses = 0;
        public static int maxMisses = 3;
        public static int usesLeft = 3;

        public static PlayerControl observed;
        public static PlayerControl currentTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.DoomsayerButton.png", 115f);
            return buttonSprite;
        }

        public static List<RoleInfo> Killing = new() {
            RoleInfo.timespaceManager,
            RoleInfo.bountyHunter,
            RoleInfo.witch,
            RoleInfo.jekyllAndHyde,
            RoleInfo.thief,
            RoleInfo.serialKiller
        };

        public static List<RoleInfo> Trick = new()
        {
            RoleInfo.nekoKabocha,
            RoleInfo.ninja,
            RoleInfo.veteran,
            RoleInfo.niceSwapper,
            RoleInfo.evilSwapper,
            RoleInfo.mayor,
            RoleInfo.bait
        };

        public static List<RoleInfo> Detect = new()
        {
            RoleInfo.snitch,
            RoleInfo.fortuneTeller,
            RoleInfo.hacker,
            RoleInfo.evilHacker,
            RoleInfo.blackmailer,
            RoleInfo.archaeologist
        };

        public static List<RoleInfo> Body = new()
        {
            RoleInfo.sherlock,
            RoleInfo.vulture,
            RoleInfo.cleaner,
            RoleInfo.undertaker,
            RoleInfo.vampire
        };

        public static List<RoleInfo> Panic = new()
        {
            RoleInfo.teleporter,
            RoleInfo.yasuna,
            RoleInfo.evilYasuna,
            RoleInfo.fortuneTeller,
            RoleInfo.trapper,
            RoleInfo.plagueDoctor,
            RoleInfo.eraser,
            RoleInfo.morphling
        };

        public static List<RoleInfo> Team = new()
        {
            RoleInfo.bomberA,
            RoleInfo.bomberB,
            RoleInfo.jackal,
            RoleInfo.sidekick,
            RoleInfo.fox,
            RoleInfo.immoralist,
            RoleInfo.mimicK,
            RoleInfo.mimicA,
        };

        public static List<RoleInfo> Protection = new()
        {
            RoleInfo.akujo,
            RoleInfo.cupid,
            RoleInfo.medic,
            RoleInfo.engineer,
            RoleInfo.camouflager,
            RoleInfo.securityGuard
        };

        public static List<RoleInfo> Outlook = new()
        {
            RoleInfo.schrodingersCat,
            RoleInfo.moriarty,
            RoleInfo.warlock,
            RoleInfo.seer,
            RoleInfo.taskMaster,
            RoleInfo.timeMaster,
            RoleInfo.portalmaker
        };

        public static List<RoleInfo> Hunting = new()
        {
            RoleInfo.tracker,
            RoleInfo.evilTracker,
            RoleInfo.detective,
            RoleInfo.sprinter,
            RoleInfo.kataomoi,
            RoleInfo.assassin,
            RoleInfo.noisemaker,
            RoleInfo.lighter,
            RoleInfo.yoyo
        };

        public static void clearAndReload()
        {
            doomsayer = null;
            counter = 0;
            triggerWin = false;
            failedGuesses = 0;
            cooldown = CustomOptionHolder.doomsayerObserveCooldown.getFloat();
            guessesToWin = Mathf.RoundToInt(CustomOptionHolder.doomsayerGuessesToWin.getFloat());
            hasMultipleGuesses = CustomOptionHolder.doomsayerMultipleGuesses.getBool();
            canObserve = CustomOptionHolder.doomsayerCanObserve.getBool();
            maxMisses = Mathf.RoundToInt(CustomOptionHolder.doomsayerMaxMisses.getFloat());
            indicateGuesses = CustomOptionHolder.doomsayerIndicator.getBool();
            usesLeft = canObserve ? Mathf.RoundToInt(CustomOptionHolder.doomsayerNumberOfObserves.getFloat()) : 0;
        }
    }
}
