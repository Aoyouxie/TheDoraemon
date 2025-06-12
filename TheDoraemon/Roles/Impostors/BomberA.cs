using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Utilities;
using TheDoraemon.Patches;



namespace TheDoraemon.Roles.Impostors
{
    public static class BomberA
    {
        public static PlayerControl bomberA;
        public static Color color = Palette.ImpostorRed;

        public static PlayerControl bombTarget;
        public static PlayerControl currentTarget;
        public static PlayerControl tmpTarget;

        public static Sprite bomberButtonSprite;
        public static Sprite releaseButtonSprite;
        public static float updateTimer = 0f;
        public static List<Arrow> arrows = new();
        public static float arrowUpdateInterval = 0.5f;

        public static float duration;
        public static float cooldown;
        public static bool countAsOne;
        public static bool showEffects;
        public static bool ifOneDiesBothDie;
        public static bool hasOneVote;
        public static bool alwaysShowArrow;

        public static TMPro.TextMeshPro targetText;
        public static TMPro.TextMeshPro partnerTargetText;
        public static Dictionary<byte, PoolablePlayer> playerIcons = new();

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

        public static void arrowUpdate()
        {
            if (bomberA.Data.IsDead || BomberB.bomberB == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }
            if ((bombTarget == null || BomberB.bombTarget == null) && !alwaysShowArrow) return;
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
                /*if (BomberB.bomberB == null || BomberB.bomberB.Data.IsDead) return;
                // 相方の位置を示すArrowsを描画
                Arrow arrow = new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(BomberB.bomberB.transform.position);
                arrows.Add(arrow);*/
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    if (p == BomberB.bomberB)
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

        public static void playerIconsUpdate()
        {
            foreach (PoolablePlayer pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
            foreach (PoolablePlayer pp in playerIcons.Values) pp.gameObject.SetActive(false);
            if (bomberA != null && BomberB.bomberB != null && !BomberB.bomberB.Data.IsDead && !bomberA.Data.IsDead && !MeetingHud.Instance)
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
                if (BomberB.bombTarget != null && playerIcons.ContainsKey(BomberB.bombTarget.PlayerId) && playerIcons[BomberB.bombTarget.PlayerId].gameObject != null)
                {
                    var icon = playerIcons[BomberB.bombTarget.PlayerId];
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

        public static void clearPlayerIcons()
        {
            foreach (PoolablePlayer p in playerIcons.Values)
            {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
        }

        public static void clearAndReload()
        {
            bomberA = null;
            bombTarget = null;
            currentTarget = null;
            tmpTarget = null;
            clearPlayerIcons();
            playerIcons = new Dictionary<byte, PoolablePlayer>();
            TORMapOptions.resetPoolables();
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            targetText = null;
            partnerTargetText = null;

            duration = CustomOptionHolder.bomberDuration.getFloat();
            cooldown = CustomOptionHolder.bomberCooldown.getFloat();
            countAsOne = CustomOptionHolder.bomberCountAsOne.getBool();
            showEffects = CustomOptionHolder.bomberShowEffects.getBool();
            hasOneVote = CustomOptionHolder.bomberHasOneVote.getBool();
            ifOneDiesBothDie = CustomOptionHolder.bomberIfOneDiesBothDie.getBool();
            alwaysShowArrow = CustomOptionHolder.bomberAlwaysShowArrow.getBool();
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
}
    