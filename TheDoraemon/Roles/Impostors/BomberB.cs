using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Utilities;
using TheDoraemon.Patches;
namespace TheDoraemon.Roles.Impostors
{
    public static class BomberB
    {
        public static PlayerControl bomberB;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl bombTarget;
        public static PlayerControl tmpTarget;
        public static PlayerControl currentTarget;
        public static TMPro.TextMeshPro targetText;
        public static TMPro.TextMeshPro partnerTargetText;
        public static Dictionary<byte, PoolablePlayer> playerIcons = new();
        public static Sprite bomberButtonSprite;
        public static Sprite releaseButtonSprite;
        public static float updateTimer = 0f;
        public static List<Arrow> arrows = new();
        public static float arrowUpdateInterval = 0.5f;

        public static void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            foreach (PoolablePlayer pp in playerIcons.Values) pp.gameObject.SetActive(false);
            if (BomberA.bomberA != null && bomberB != null && !bomberB.Data.IsDead && !BomberA.bomberA.Data.IsDead && !MeetingHud.Instance)
            {
                if (bombTarget != null && TORMapOptions.playerIcons.ContainsKey(bombTarget.PlayerId) && TORMapOptions.playerIcons[bombTarget.PlayerId].gameObject != null)
                {
                    var icon = TORMapOptions.playerIcons[bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (targetText == null)
                    {
                        targetText = Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        targetText.enableWordWrapping = false;
                        targetText.transform.localScale = Vector3.one * 1.5f;
                        targetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    targetText.text = ModTranslation.getString("bomberYourTarget");
                    targetText.gameObject.SetActive(true);
                    targetText.transform.SetParent(icon.gameObject.transform);
                }
                // 相方の設置したターゲットを表示する
                if (BomberA.bombTarget != null && playerIcons.ContainsKey(BomberA.bombTarget.PlayerId) && playerIcons[BomberA.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = playerIcons[BomberA.bombTarget.PlayerId];
                    Vector3 bottomLeft = new Vector3(-0.82f, 0.19f, 0) + IntroCutsceneOnDestroyPatch.bottomLeft;
                    icon.gameObject.SetActive(true);
                    icon.transform.localPosition = bottomLeft + new Vector3(1.0f, 0f, 0);
                    icon.transform.localScale = Vector3.one * 0.4f;
                    if (partnerTargetText == null)
                    {
                        partnerTargetText = Object.Instantiate(icon.cosmetics.nameText, icon.cosmetics.nameText.transform.parent);
                        partnerTargetText.enableWordWrapping = false;
                        partnerTargetText.transform.localScale = Vector3.one * 1.5f;
                        partnerTargetText.transform.localPosition += new Vector3(0f, 1.7f, 0);
                    }
                    partnerTargetText.text = ModTranslation.getString("bomberPartnerTarget");
                    partnerTargetText.gameObject.SetActive(true);
                    partnerTargetText.transform.SetParent(icon.gameObject.transform);
                }
            }
        }

        public static void arrowUpdate()
        {
            if (bomberB.Data.IsDead || BomberA.bomberA == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }
            if ((BomberA.bombTarget == null || bombTarget == null) && !BomberA.alwaysShowArrow) return;
            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (p == BomberA.bomberA)
                    {
                        Arrow arrow;
                        arrow = new Arrow(Color.red);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }
                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static Sprite getBomberButtonSprite()
        {
            if (bomberButtonSprite) return bomberButtonSprite;
            bomberButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.PlantBombButton.png", 115f);
            return bomberButtonSprite;
        }
        public static Sprite getReleaseButtonSprite()
        {
            if (releaseButtonSprite) return releaseButtonSprite;
            releaseButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.ReleaseButton.png", 115f);
            return releaseButtonSprite;
        }

        public static void clearPlayerIcons()
        {
            foreach (PoolablePlayer p in playerIcons.Values) {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
        }

        public static void clearAndReload()
        {
            bomberB = null;
            bombTarget = null;
            currentTarget = null;
            tmpTarget = null;
            TORMapOptions.resetPoolables();
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            clearPlayerIcons();
            playerIcons = new Dictionary<byte, PoolablePlayer>();
            targetText = null;
            partnerTargetText = null;
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatchForBomber
        {
            public static void Prefix(IntroCutscene __instance)
            {
                if (PlayerControl.LocalPlayer != null && FastDestroyableSingleton<HudManager>.Instance != null)
                {
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                    {
                        NetworkedPlayerInfo data = p.Data;
                        PoolablePlayer player = Object.Instantiate(__instance.PlayerPrefab, FastDestroyableSingleton<HudManager>.Instance.transform);
                        p.SetPlayerMaterialColors(player.cosmetics.currentBodySprite.BodySprite);
                        player.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                        player.cosmetics.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                        player.cosmetics.nameText.text = data.PlayerName;
                        player.SetFlipX(true);
                        player.gameObject.SetActive(false);
                        playerIcons[p.PlayerId] = player;
                    }
                }
            }
        }
    }

    /*public static class Shifter {
        public static PlayerControl shifter;

        public static PlayerControl futureShift;
        public static PlayerControl currentTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static void shiftRole (PlayerControl player1, PlayerControl player2, bool repeat = true) {
            if (Mayor.mayor != null && Mayor.mayor == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Mayor.mayor = player1;
            } else if (Portalmaker.portalmaker != null && Portalmaker.portalmaker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Portalmaker.portalmaker = player1;
            } else if (Engineer.engineer != null && Engineer.engineer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Engineer.engineer = player1;
            } else if (TimespaceManager.timespaceManager != null && TimespaceManager.timespaceManager == player2) {
                if (repeat) shiftRole(player2, player1, false);
                if (TimespaceManager.formerDeputy != null && TimespaceManager.formerDeputy == TimespaceManager.timespaceManager) TimespaceManager.formerDeputy = player1;  // Shifter also shifts info on promoted deputy (to get handcuffs)
                TimespaceManager.timespaceManager = player1;
            } else if (Deputy.deputy != null && Deputy.deputy == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Deputy.deputy = player1;
            } else if (Lighter.lighter != null && Lighter.lighter == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Lighter.lighter = player1;
            } else if (Detective.detective != null && Detective.detective == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Detective.detective = player1;
            } else if (TimeMaster.timeMaster != null && TimeMaster.timeMaster == player2) {
                if (repeat) shiftRole(player2, player1, false);
                TimeMaster.timeMaster = player1;
            }  else if (Medic.medic != null && Medic.medic == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Medic.medic = player1;
            } else if (Swapper.swapper != null && Swapper.swapper == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Swapper.swapper = player1;
            } else if (Seer.seer != null && Seer.seer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Seer.seer = player1;
            } else if (Hacker.hacker != null && Hacker.hacker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Hacker.hacker = player1;
            } else if (Tracker.tracker != null && Tracker.tracker == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Tracker.tracker = player1;
            } else if (Snitch.snitch != null && Snitch.snitch == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Snitch.snitch = player1;
            } else if (Spy.spy != null && Spy.spy == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Spy.spy = player1;
            } else if (SecurityGuard.securityGuard != null && SecurityGuard.securityGuard == player2) {
                if (repeat) shiftRole(player2, player1, false);
                SecurityGuard.securityGuard = player1;
            } else if (Guesser.niceGuesser != null && Guesser.niceGuesser == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Guesser.niceGuesser = player1;
            } else if (Medium.medium != null && Medium.medium == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Medium.medium = player1;
            } else if (Pursuer.pursuer != null && Pursuer.pursuer == player2) {
                if (repeat) shiftRole(player2, player1, false);
                Pursuer.pursuer = player1;
            } //else if (Trapper.trapper != null && Trapper.trapper == player2) {
                //if (repeat) shiftRole(player2, player1, false);
                //Trapper.trapper = player1;
            //}
        }

        public static void clearAndReload() {
            shifter = null;
            currentTarget = null;
            futureShift = null;
        }
    }*/
}
    