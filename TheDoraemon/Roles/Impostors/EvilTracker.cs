using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Utilities;
using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Neutrals;
namespace TheDoraemon.Roles.Impostors
{
    public static class EvilTracker
    {
        public static PlayerControl evilTracker;
        public static Color color = Palette.ImpostorRed;

        public static float cooldown = 10f;
        public static bool resetTargetAfterMeeting = true;
        public static bool canSeeDeathFlash = true;
        public static bool canSeeTargetPosition = true;
        public static bool canSetTargetOnMeeting = true;
        public static bool canSeeTargetTasks = false;

        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;

        public static PlayerControl target;
        public static PlayerControl futureTarget;
        public static PlayerControl currentTarget;
        public static Sprite trackerButtonSprite;
        public static Sprite arrowSprite;
        public static List<Arrow> arrows = new();
        public static Dictionary<string, TMPro.TMP_Text> impostorPositionText;
        public static TMPro.TMP_Text targetPositionText;

        public static Sprite getEvilTrackerButtonSprite()
        {
            if (trackerButtonSprite) return trackerButtonSprite;
            trackerButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.TrackerButton.png", 115f);
            return trackerButtonSprite;
        }

        public static Sprite getArrowSprite()
        {
            if (!arrowSprite)
                arrowSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.Arrow.png", 300f);
            return arrowSprite;
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (arrows?.Count > 0) {
                    foreach (var arrow in arrows)
                        if (arrow != null && arrow.arrow != null) Object.Destroy(arrow.arrow);
                }
                if (impostorPositionText.Count > 0) {
                foreach (var p in impostorPositionText.Values)
                    if (p != null) Object.Destroy(p.gameObject);
                }
                if (targetPositionText != null) Object.Destroy(targetPositionText.gameObject);
                target = null;
                return;
            }

            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow in arrows)
                {
                    if (arrow != null && arrow.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();

                // インポスターの位置を示すArrowsを描画
                int count = 0;
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead)
                    {
                        if ((p.Data.Role.IsImpostor || p == Sidekick.sidekick && Sidekick.wasTeamRed
                        || p == Jackal.jackal && Jackal.wasTeamRed) && impostorPositionText.ContainsKey(p.Data.PlayerName))
                        {
                            impostorPositionText[p.Data.PlayerName].text = "";
                        }
                        continue;
                    }
                    Arrow arrow;
                    if (p.Data.Role.IsImpostor && p != PlayerControl.LocalPlayer|| p == Sidekick.sidekick && Sidekick.wasTeamRed
                        || p == Jackal.jackal && Jackal.wasTeamRed)
                    {
                        arrow = new Arrow(Palette.ImpostorRed);
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                        count += 1;
                        if (!impostorPositionText.ContainsKey(p.Data.PlayerName))
                        {
                            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                            if (roomTracker == null) return;
                            GameObject gameObject = Object.Instantiate(roomTracker.gameObject);
                            Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                            gameObject.transform.localPosition = new Vector3(0, -2.0f + 0.25f * count, gameObject.transform.localPosition.z);
                            gameObject.transform.localScale = Vector3.one * 1.0f;
                            TMPro.TMP_Text positionText = gameObject.GetComponent<TMPro.TMP_Text>();
                            positionText.alpha = 1.0f;
                            impostorPositionText.Add(p.Data.PlayerName, positionText);
                        }
                        PlainShipRoom room = Helpers.getPlainShipRoom(p);
                        impostorPositionText[p.Data.PlayerName].gameObject.SetActive(true);
                        if (room != null)
                        {
                            impostorPositionText[p.Data.PlayerName].text = "<color=#FF1919FF>" + $"{p.Data.PlayerName}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                        }
                        else
                        {
                            impostorPositionText[p.Data.PlayerName].text = "";
                        }
                    }
                }

                // ターゲットの位置を示すArrowを描画
                if (target != null && !target.Data.IsDead)
                {
                    Arrow arrow = new(Palette.CrewmateBlue);
                    arrow.arrow.SetActive(true);
                    arrow.Update(target.transform.position);
                    arrows.Add(arrow);
                    if (targetPositionText == null)
                    {
                        RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = Object.Instantiate(roomTracker.gameObject);
                        Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        targetPositionText.alpha = 1.0f;
                    }
                    PlainShipRoom room = Helpers.getPlainShipRoom(target);
                    targetPositionText.gameObject.SetActive(true);
                    if (room != null)
                    {
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}(" + FastDestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    }
                    else
                    {
                        targetPositionText.text = "";
                    }
                }
                else
                {
                    if (targetPositionText != null)
                    {
                        targetPositionText.text = "";
                    }
                }

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void clearAllArrow()
        {
            if (PlayerControl.LocalPlayer != evilTracker) return;
            if (arrows?.Count > 0) {
            foreach (var arrow in arrows)
                if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            }
            if (impostorPositionText.Count > 0) {
                foreach (var p in impostorPositionText.Values)
                    if (p != null) p.gameObject.SetActive(false);
            }
            if (targetPositionText != null) targetPositionText.gameObject.SetActive(false);
        }

        public static void clearAndReload()
        {
            evilTracker = null;
            target = null;
            futureTarget = null;
            currentTarget = null;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            if (impostorPositionText != null)
            {
                foreach (var p in impostorPositionText.Values)
                    if (p != null)
                        Object.Destroy(p);
            }
            impostorPositionText = new();
            if (targetPositionText != null) Object.Destroy(targetPositionText);
            targetPositionText = null;

            cooldown = CustomOptionHolder.evilTrackerCooldown.getFloat();
            resetTargetAfterMeeting = CustomOptionHolder.evilTrackerResetTargetAfterMeeting.getBool();
            canSeeDeathFlash = CustomOptionHolder.evilTrackerCanSeeDeathFlash.getBool();
            canSeeTargetPosition = CustomOptionHolder.evilTrackerCanSeeTargetPosition.getBool();
            canSeeTargetTasks = CustomOptionHolder.evilTrackerCanSeeTargetTask.getBool();
            canSetTargetOnMeeting = CustomOptionHolder.evilTrackerCanSetTargetOnMeeting.getBool();
        }
    }
}
    