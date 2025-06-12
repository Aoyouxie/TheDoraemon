using HarmonyLib;
using System;
using TheDoraemon.Utilities;
using TheDoraemon.CustomGameModes;
using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Impostors;
using TheDoraemon.Roles.Neutrals;
using TheDoraemon.Roles.Modifier;


namespace TheDoraemon
{
    [HarmonyPatch]
    public static class TheDoraemon
    {

        public static System.Random rnd = new((int)DateTime.Now.Ticks);

        public static void clearAndReloadRoles()
        {
            Jester.clearAndReload();
            Mayor.clearAndReload();
            Portalmaker.clearAndReload();
            Engineer.clearAndReload();
            Deputy.clearAndReload();
            Lighter.clearAndReload();
            Godfather.clearAndReload();
            Mafioso.clearAndReload();
            Janitor.clearAndReload();
            Detective.clearAndReload();
            TimeMaster.clearAndReload();
            Medic.clearAndReload();
            Shifter.clearAndReload();
            Swapper.clearAndReload();
            Lovers.clearAndReload();
            Seer.clearAndReload();
            Bait.clearAndReload();
            Morphling.clearAndReload();
            Camouflager.clearAndReload();
            Hacker.clearAndReload();
            Tracker.clearAndReload();
            Vampire.clearAndReload();
            Snitch.clearAndReload();
            Jackal.clearAndReload();
            Sidekick.clearAndReload();
            Eraser.clearAndReload();
            Trickster.clearAndReload();
            Cleaner.clearAndReload();
            Warlock.clearAndReload();
            SecurityGuard.clearAndReload();
            Arsonist.clearAndReload();
            BountyHunter.clearAndReload();
            Vulture.clearAndReload();
            Pursuer.clearAndReload();
            Witch.clearAndReload();
            Assassin.clearAndReload();
            Thief.clearAndReload();
            Ninja.clearAndReload();
            NekoKabocha.clearAndReload();
            SerialKiller.clearAndReload();
            EvilTracker.clearAndReload();
            Undertaker.clearAndReload();
            MimicK.clearAndReload();
            MimicA.clearAndReload();
            BomberA.clearAndReload();
            BomberB.clearAndReload();
            EvilHacker.clearAndReload();
            Trapper.clearAndReload();
            Blackmailer.clearAndReload();
            Yoyo.clearAndReload();
            LastImpostor.clearAndReload();
            FortuneTeller.clearAndReload();
            Sprinter.clearAndReload();
            Veteran.clearAndReload();
            Sherlock.clearAndReload();
            TaskMaster.clearAndReload();
            Yasuna.clearAndReload();
            Madmate.clearAndReload();
            CreatedMadmate.clearAndReload();
            Teleporter.clearAndReload();
            Noisemaker.clearAndReload();
            Archaeologist.clearAndReload();
            Watcher.clearAndReload();
            Opportunist.clearAndReload();
            Moriarty.clearAndReload();
            Akujo.clearAndReload();
            PlagueDoctor.clearAndReload();
            JekyllAndHyde.clearAndReload();
            Cupid.clearAndReload();
            Fox.clearAndReload();
            Immoralist.clearAndReload();
            SchrodingersCat.clearAndReload();
            Kataomoi.clearAndReload();
            Doomsayer.clearAndReload();
            Husk.clearAndReload();

            // Modifier
            //Bait.clearAndReload();
            Bloody.clearAndReload();
            AntiTeleport.clearAndReload();
            Tiebreaker.clearAndReload();
            Sunglasses.clearAndReload();
            Vip.clearAndReload();
            Invert.clearAndReload();
            Chameleon.clearAndReload();
            Armored.clearAndReload();

            // Gamemodes
            HandleGuesser.clearAndReload();
            HideNSeek.clearAndReload();
            FreePlayGM.clearAndReload();

            //以下职业使用了GMIA代码
            TimespaceManager.clearAndReload();
        }
    }
}
