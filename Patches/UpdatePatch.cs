using HarmonyLib;
using System;
using UnityEngine;

using TheDoraemon.Objects;
using System.Collections.Generic;
using System.Linq;
using TheDoraemon.Utilities;
using TheDoraemon.CustomGameModes;
using AmongUs.GameOptions;
using TheDoraemon.Modules;
using static TheDoraemon.GameHistory;
using Hazel;

using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Impostors;
using TheDoraemon.Roles.Neutrals;
using TheDoraemon.Roles.Modifier;



namespace TheDoraemon.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        private static Dictionary<byte, (string name, Color color)> TagColorDict = new();
        static void resetNameTagsAndColors() {
            var localPlayer = PlayerControl.LocalPlayer;
            var myData = PlayerControl.LocalPlayer.Data;
            var amImpostor = myData.Role.IsImpostor;
            var morphTimerNotUp = Morphling.morphTimer > 0f;
            var morphTargetNotNull = Morphling.morphTarget != null;

            var dict = TagColorDict;
            dict.Clear();
            
            foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                var player = data.Object;
                string text = data.PlayerName;
                Color color;
                if (player)
                {
                    var playerName = text;
                    if (morphTimerNotUp && morphTargetNotNull && Morphling.morphling == player) playerName = Morphling.morphTarget.Data.PlayerName;
                    if (MimicA.isMorph && MimicA.mimicA == player && MimicA.mimicA != null && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead) playerName = MimicK.mimicK.Data.PlayerName;
                    if (MimicK.mimicK != null && MimicK.victim != null && MimicK.mimicK == player) playerName = MimicK.victim.Data.PlayerName;
                    var nameText = player.cosmetics.nameText;
                
                    nameText.text = Helpers.hidePlayerName(localPlayer, player) ? "" : playerName;
                    nameText.color = color = amImpostor && data.Role.IsImpostor ? Palette.ImpostorRed : Color.white;
                    nameText.color = nameText.color.SetAlpha(Chameleon.visibility(player.PlayerId));
                }
                else
                {
                    color = Color.white;
                }
                
                
                dict.Add(data.PlayerId, (text, color));
            }
            
            if (MeetingHud.Instance != null) 
            {
                foreach (PlayerVoteArea playerVoteArea in MeetingHud.Instance.playerStates)
                {
                    var data = dict[playerVoteArea.TargetPlayerId];
                    var text = playerVoteArea.NameText;
                    text.text = data.name;
                    text.color = data.color;
                }
            }
        }

        static void setPlayerNameColor(PlayerControl p, Color color) {
            p.cosmetics.nameText.color = color.SetAlpha(Chameleon.visibility(p.PlayerId));
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = color;
        }

        static void setNameColors() {
            var localPlayer = PlayerControl.LocalPlayer;
            var localRole = RoleInfo.getRoleInfoForPlayer(localPlayer, false).FirstOrDefault();
            setPlayerNameColor(localPlayer, localRole.color);

            /*if (Jester.jester != null && Jester.jester == localPlayer)
                setPlayerNameColor(Jester.jester, Jester.color);
            else if (Mayor.mayor != null && Mayor.mayor == localPlayer)
                setPlayerNameColor(Mayor.mayor, Mayor.color);
            else if (Engineer.engineer != null && Engineer.engineer == localPlayer)
                setPlayerNameColor(Engineer.engineer, Engineer.color);
            else if (TimespaceManager.timespaceManager != null && TimespaceManager.timespaceManager == localPlayer) {
                setPlayerNameColor(TimespaceManager.timespaceManager, TimespaceManager.color);
                if (Deputy.deputy != null && Deputy.knowsSheriff) {
                    setPlayerNameColor(Deputy.deputy, Deputy.color);
                }
            } else*/
            if (Deputy.deputy != null && Deputy.deputy == localPlayer) 
            {
                setPlayerNameColor(Deputy.deputy, Deputy.color);
                if (TimespaceManager.timespaceManager != null && Deputy.knowsSheriff) {
                    setPlayerNameColor(TimespaceManager.timespaceManager, TimespaceManager.color);
                }
            }
            else if (TimespaceManager.timespaceManager != null && TimespaceManager.timespaceManager == localPlayer)
            {
                setPlayerNameColor(TimespaceManager.timespaceManager, TimespaceManager.color);
                if (Deputy.deputy != null && Deputy.knowsSheriff) setPlayerNameColor(Deputy.deputy, TimespaceManager.color);
            }
            /*else if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == localPlayer)
                setPlayerNameColor(Portalmaker.portalmaker, Portalmaker.color);
            else if (Lighter.lighter != null && Lighter.lighter == localPlayer)
                setPlayerNameColor(Lighter.lighter, Lighter.color);
            else if (Detective.detective != null && Detective.detective == localPlayer)
                setPlayerNameColor(Detective.detective, Detective.color);
            else if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == localPlayer)
                setPlayerNameColor(TimeMaster.timeMaster, TimeMaster.color);
            else if (Medic.medic != null && Medic.medic == localPlayer)
                setPlayerNameColor(Medic.medic, Medic.color);
            else if (Shifter.shifter != null && Shifter.shifter == localPlayer)
                setPlayerNameColor(Shifter.shifter, Shifter.color);
            else if (Swapper.swapper != null && Swapper.swapper == localPlayer)
                setPlayerNameColor(Swapper.swapper, Swapper.color);
            else if (Seer.seer != null && Seer.seer == localPlayer)
                setPlayerNameColor(Seer.seer, Seer.color);
            else if (Hacker.hacker != null && Hacker.hacker == localPlayer)
                setPlayerNameColor(Hacker.hacker, Hacker.color);
            else if (Tracker.tracker != null && Tracker.tracker == localPlayer)
                setPlayerNameColor(Tracker.tracker, Tracker.color);
            else if (Snitch.snitch != null && Snitch.snitch == localPlayer)
                setPlayerNameColor(Snitch.snitch, Snitch.color);*/
            else if (Jackal.jackal != null && Jackal.jackal == localPlayer) {
                // Jackal can see his sidekick
                setPlayerNameColor(Jackal.jackal, Jackal.color);
                if (Sidekick.sidekick != null) {
                    setPlayerNameColor(Sidekick.sidekick, Jackal.color);
                }
                if (Jackal.fakeSidekick != null) {
                    setPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
                }
                if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, Jackal.color);
            }
            else if (FortuneTeller.fortuneTeller != null && FortuneTeller.fortuneTeller == localPlayer && (FortuneTeller.isCompletedNumTasks(PlayerControl.LocalPlayer) || PlayerControl.LocalPlayer.Data.IsDead))
            {
                setPlayerNameColor(FortuneTeller.fortuneTeller, FortuneTeller.color);
            }
            else if (TaskMaster.taskMaster != null && TaskMaster.taskMaster == localPlayer)
            {
                setPlayerNameColor(TaskMaster.taskMaster, !TaskMaster.becomeATaskMasterWhenCompleteAllTasks || TaskMaster.isTaskComplete ? TaskMaster.color : RoleInfo.crewmate.color);
            }
            else if (Swapper.swapper != null && Swapper.swapper == localPlayer)
            {
                setPlayerNameColor(Swapper.swapper, Swapper.swapper.Data.Role.IsImpostor ? Palette.ImpostorRed : Swapper.color);
            }
            else if (Yasuna.yasuna != null && Yasuna.yasuna == localPlayer)
            {
                setPlayerNameColor(Yasuna.yasuna, localPlayer.Data.Role.IsImpostor ? Palette.ImpostorRed : Yasuna.color);
            }
            else if (Kataomoi.kataomoi != null && Kataomoi.kataomoi == localPlayer)
            {
                setPlayerNameColor(Kataomoi.kataomoi, Kataomoi.color);
                if (Kataomoi.target != null)
                    setPlayerNameColor(Kataomoi.target, Kataomoi.color);
            }

            // No else if here, as a Lover of team Jackal needs the colors
            if (Sidekick.sidekick != null && Sidekick.sidekick == localPlayer) {
                // Sidekick can see the jackal
                setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
                if (Jackal.jackal != null) {
                    setPlayerNameColor(Jackal.jackal, Jackal.color);
                }
                if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, Jackal.color);
            }

            if (SchrodingersCat.schrodingersCat != null && localPlayer == SchrodingersCat.schrodingersCat)
            {
                if (SchrodingersCat.team == SchrodingersCat.Team.Impostor)
                {
                    foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p.Data.Role.IsImpostor) setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
                else if (SchrodingersCat.team == SchrodingersCat.Team.Jackal)
                {
                    if (Jackal.jackal != null) setPlayerNameColor(Jackal.jackal, Jackal.color);
                    if (Sidekick.sidekick != null) setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
                }
                else if (SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde && JekyllAndHyde.jekyllAndHyde != null)
                    setPlayerNameColor(JekyllAndHyde.jekyllAndHyde, JekyllAndHyde.color);
                else if (SchrodingersCat.team == SchrodingersCat.Team.Moriarty && Moriarty.moriarty != null)
                    setPlayerNameColor(Moriarty.moriarty, Moriarty.color);
            }

            if (SchrodingersCat.schrodingersCat != null)
            {
                if (localPlayer == JekyllAndHyde.jekyllAndHyde && SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, JekyllAndHyde.color);
                if (localPlayer == Moriarty.moriarty && SchrodingersCat.team == SchrodingersCat.Team.Moriarty)
                    setPlayerNameColor(SchrodingersCat.schrodingersCat, Moriarty.color);
            }

            // No else if here, as the Impostors need the Spy name to be colored
            if (localPlayer.Data.Role.IsImpostor)
            {
            }
            if (Sidekick.sidekick != null && Sidekick.wasTeamRed && localPlayer.Data.Role.IsImpostor)
            {
          
            }
            if (Jackal.jackal != null && Jackal.wasTeamRed && localPlayer.Data.Role.IsImpostor)
            {

            }
            if (Fox.fox != null && localPlayer == Fox.fox) { 
                setPlayerNameColor(localPlayer, Fox.color);
                if (Immoralist.immoralist != null) { 
                    setPlayerNameColor(Immoralist.immoralist, Immoralist.color);
                }
            }
            if (Immoralist.immoralist != null && localPlayer == Immoralist.immoralist) {
                setPlayerNameColor(localPlayer, Immoralist.color);
                if (Fox.fox != null) { 
                    setPlayerNameColor(Fox.fox, Immoralist.color);
                }
            }
            if (Madmate.madmate.Contains(localPlayer))
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Madmate.color);
                if (Madmate.tasksComplete(localPlayer))
                {
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p.Data.Role.IsImpostor || (p == Jackal.jackal && Jackal.wasTeamRed) || (p == Sidekick.sidekick && Sidekick.wasTeamRed))
                        {
                            setPlayerNameColor(p, Palette.ImpostorRed);
                        }
                    }
                }
            }
            if (localPlayer == CreatedMadmate.createdMadmate)
            {
                setPlayerNameColor(PlayerControl.LocalPlayer, Madmate.color);
                if (CreatedMadmate.tasksComplete(localPlayer))
                {
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p.Data.Role.IsImpostor || (p == Jackal.jackal && Jackal.wasTeamRed) || (p == Sidekick.sidekick && Sidekick.wasTeamRed))
                        {
                            setPlayerNameColor(p, Palette.ImpostorRed);
                        }
                    }
                }
            }
            // Crewmate roles with no changes: Mini
            // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
        }

        static void setNameTags() {
            // Mafia
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (Godfather.godfather != null && Godfather.godfather == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + $" ({ModTranslation.getString("mafiaG")})";
                    else if (Mafioso.mafioso != null && Mafioso.mafioso == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + $" ({ModTranslation.getString("mafiaM")})";
                    else if (Janitor.janitor != null && Janitor.janitor == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + $" ({ModTranslation.getString("mafiaJ")})";
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Godfather.godfather != null && Godfather.godfather.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Godfather.godfather.Data.PlayerName + $" ({ModTranslation.getString("mafiaG")})";
                        else if (Mafioso.mafioso != null && Mafioso.mafioso.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Mafioso.mafioso.Data.PlayerName + $" ({ModTranslation.getString("mafiaM")})";
                        else if (Janitor.janitor != null && Janitor.janitor.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Janitor.janitor.Data.PlayerName + $" ({ModTranslation.getString("mafiaJ")})";
            }

            // Lovers
            if (Lovers.lover1 != null && Lovers.lover2 != null && (Lovers.lover1 == PlayerControl.LocalPlayer || Lovers.lover2 == PlayerControl.LocalPlayer)) {
                string suffix = Helpers.cs(Lovers.color, " ♥");
                Lovers.lover1.cosmetics.nameText.text += suffix;
                Lovers.lover2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Lovers.lover1.PlayerId == player.TargetPlayerId || Lovers.lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Cupid
            if (Cupid.lovers1 != null && Cupid.lovers2 != null && (Cupid.lovers1 == PlayerControl.LocalPlayer || Cupid.lovers2 == PlayerControl.LocalPlayer || (Cupid.cupid != null && PlayerControl.LocalPlayer == Cupid.cupid)))
            {
                string suffix = Helpers.cs(Cupid.color, " ♥");
                Cupid.lovers1.cosmetics.nameText.text += suffix;
                Cupid.lovers2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Cupid.lovers1.PlayerId == player.TargetPlayerId || Cupid.lovers2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Kataomoi
            if (Kataomoi.kataomoi != null && Kataomoi.kataomoi == PlayerControl.LocalPlayer && Kataomoi.target != null)
            {
                string suffix = Helpers.cs(Kataomoi.color, " ♥");
                Kataomoi.target.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Kataomoi.target.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Akujo
            if (Akujo.akujo != null && (Akujo.keeps != null || Akujo.honmei != null))
            {
                if (Akujo.keeps != null)
                {
                    foreach (PlayerControl p in Akujo.keeps)
                    {
                        if (PlayerControl.LocalPlayer == Akujo.akujo) p.cosmetics.nameText.text += Helpers.cs(Color.gray, " ♥");
                        if (PlayerControl.LocalPlayer == p)
                        {
                            Akujo.akujo.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                            p.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                        }
                    }
                }
                if (Akujo.honmei != null)
                {
                    if (PlayerControl.LocalPlayer == Akujo.akujo) Akujo.honmei.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                    if (PlayerControl.LocalPlayer == Akujo.honmei)
                    {
                        Akujo.akujo.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                        Akujo.honmei.cosmetics.nameText.text += Helpers.cs(Akujo.color, " ♥");
                    }
                }

                if (MeetingHud.Instance != null)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (player.TargetPlayerId == Akujo.akujo.PlayerId && ((Akujo.honmei != null && Akujo.honmei == PlayerControl.LocalPlayer) || (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))))
                            player.NameText.text += Helpers.cs(Akujo.color, " ♥");
                        if (PlayerControl.LocalPlayer == Akujo.akujo)
                        {
                            if (player.TargetPlayerId == Akujo.honmei?.PlayerId) player.NameText.text += Helpers.cs(Akujo.color, " ♥");
                            if (Akujo.keeps != null && Akujo.keeps.Any(x => x.PlayerId == player.TargetPlayerId)) player.NameText.text += Helpers.cs(Color.gray, " ♥");
                        }
                    }
                }
            }
            // Former Thief
            if (Thief.formerThief != null && (Thief.formerThief == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)) {
                string suffix = Helpers.cs(Thief.color, " $");
                Thief.formerThief.cosmetics.nameText.text += suffix;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == Thief.formerThief.PlayerId)
                            player.NameText.text += suffix;
            }

            // Add medic shield info:
            if (MeetingHud.Instance != null && Medic.medic != null && Medic.shielded != null && Medic.shieldVisible(Medic.shielded))
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Medic.shielded.PlayerId)
                    {
                        player.NameText.text = Helpers.cs(Medic.color, "[") + player.NameText.text + Helpers.cs(Medic.color, "]");
                        // player.HighlightedFX.color = Medic.color;
                        // player.HighlightedFX.enabled = true;
                    }
            }
        }

        static void updateShielded() {
            if (Medic.shielded == null) return;

            if (Medic.shielded.Data.IsDead || Medic.medic == null || Medic.medic.Data.IsDead) {
                Medic.shielded = null;
            }
        }

        static void timerUpdate() {
            var dt = Time.deltaTime;
            Hacker.hackerTimer -= dt;
            Trickster.lightsOutTimer -= dt;
            Tracker.corpsesTrackingTimer -= dt;
            HideNSeek.timer -= dt;
            foreach (byte key in Deputy.handcuffedKnows.Keys)
                Deputy.handcuffedKnows[key] -= dt;
        }

        static void updateImpostorKillButton(HudManager __instance) {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
            if (MeetingHud.Instance) {
                __instance.KillButton.Hide();
                return;
            }
            bool enabled = true;
            if (Vampire.vampire != null && Vampire.vampire == PlayerControl.LocalPlayer)
                enabled = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                enabled = false;
            else if (Janitor.janitor != null && Janitor.janitor == PlayerControl.LocalPlayer)
                enabled = false;
            else if (MimicA.mimicA != null && MimicA.mimicA == PlayerControl.LocalPlayer && MimicK.mimicK != null && !MimicK.mimicK.Data.IsDead)
                enabled = false;
            else if (BomberA.bomberA != null && BomberA.bomberA == PlayerControl.LocalPlayer && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead)
                enabled = false;
            else if (BomberB.bomberB != null && BomberB.bomberB == PlayerControl.LocalPlayer && BomberA.bomberA != null && !BomberA.bomberA.Data.IsDead)
                enabled = false;

            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();

            if (Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
        }

        static void updateReportButton(HudManager __instance) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0 || MeetingHud.Instance) __instance.ReportButton.Hide();
            else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
        }
         
        static void updateVentButton(HudManager __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if ((Deputy.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Deputy.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) || MeetingHud.Instance || PlayerControl.LocalPlayer.roleCanUseVents() == false) __instance.ImpostorVentButton.Hide();
            else if (PlayerControl.LocalPlayer.roleCanUseVents() && !__instance.ImpostorVentButton.isActiveAndEnabled) __instance.ImpostorVentButton.Show();

            if (Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
                __instance.ImpostorVentButton.transform.localPosition = PlayerControl.LocalPlayer == Engineer.engineer ? CustomButton.ButtonPositions.lowerRowRight : CustomButton.ButtonPositions.upperRowLeft;
            if (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == PlayerControl.LocalPlayer && CreatedMadmate.canEnterVents) __instance.ImpostorVentButton.transform.localPosition = CustomButton.ButtonPositions.lowerRowRight;

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(RewiredConsts.Action.UseVent) && !PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.roleCanUseVents()) {
                __instance.ImpostorVentButton.DoClick();
            }
        }

        static void updateUseButton(HudManager __instance) {
            if (MeetingHud.Instance) __instance.UseButton.Hide();
        }

        static void updateSabotageButton(HudManager __instance) {
            if (MeetingHud.Instance || TORMapOptions.gameMode == CustomGamemodes.HideNSeek || !PlayerControl.LocalPlayer.roleCanUseSabotage()) __instance.SabotageButton.Hide();
            else if (PlayerControl.LocalPlayer.roleCanUseSabotage() && !__instance.SabotageButton.isActiveAndEnabled) __instance.SabotageButton.Show();

            if (Helpers.ShowButtons) {
                if ((CreatedMadmate.createdMadmate != null && PlayerControl.LocalPlayer == CreatedMadmate.createdMadmate && CreatedMadmate.canSabotage)
                    || (Madmate.madmate.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) && Madmate.canSabotage))
                    __instance.SabotageButton.transform.localPosition = CustomButton.ButtonPositions.upperRowCenter + __instance.UseButton.transform.localPosition;
            }
        }

        static void updateMapButton(HudManager __instance) {
            //Trapper.trapper == null || !(PlayerControl.LocalPlayer.PlayerId == Trapper.trapper.PlayerId) ||
            if ( __instance == null || __instance.MapButton.HeldButtonSprite == null) return;
            //__instance.MapButton.HeldButtonSprite.color = Trapper.playersOnMap.Any() ? Trapper.color : Color.white;
        }

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            
            EventUtility.Update();

            CustomButton.HudUpdate();
            resetNameTagsAndColors();
            setNameColors();
            updateShielded();
            setNameTags();

            // Impostors
            updateImpostorKillButton(__instance);
            // Timer updates
            timerUpdate();

            // Deputy Sabotage, Use and Vent Button Disabling
            updateReportButton(__instance);
            updateVentButton(__instance);
            // Meeting hide buttons if needed (used for the map usage, because closing the map would show buttons)
            updateSabotageButton(__instance);
            updateUseButton(__instance);
            updateMapButton(__instance);
            if (!MeetingHud.Instance) __instance.AbilityButton?.Update();
            GameStatistics.updateTimer();
            Sherlock.UpdateArrow();

            // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
            foreach (PlayerControl target in PlayerControl.AllPlayerControls) {
                var pet = target.GetPet();
                if (pet != null) {
                    pet.Visible = ((PlayerControl.LocalPlayer.Data.IsDead && target.Data.IsDead) || !target.Data.IsDead) && !target.inVent;
                }
            }
        }
    }
}
