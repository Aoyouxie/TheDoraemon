using System;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;

namespace TheDoraemon.Roles.Neutrals
{
    public static class Moriarty
    {
        public static PlayerControl moriarty;
        public static PlayerControl formerMoriarty;
        public static Color color = Color.green;

        public static PlayerControl tmpTarget;
        public static PlayerControl target;
        public static PlayerControl currentTarget;
        public static PlayerControl killTarget;
        public static List<PlayerControl> brainwashed = new();

        public static int counter;

        public static float brainwashTime = 2f;
        public static float brainwashCooldown = 30f;
        public static int numberToWin = 3;
        public static bool indicateKills = false;
        public static bool hasKilled = false;

        public static Sprite brainwashIcon;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static TMPro.TMP_Text targetPositionText;

        public static bool triggerMoriartyWin = false;

        public static Sprite getBrainwashIcon()
        {
            if (brainwashIcon) return brainwashIcon;
            brainwashIcon = Helpers.loadSpriteFromResources("TheDoraemon.Resources.BrainwashButton.png", 115f);
            return brainwashIcon;
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (arrows.Count > 0)
                {
                    foreach (var arrow in arrows)
                        if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                }
                if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText.gameObject);
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
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();
                // ターゲットの位置を示すArrowを描画
                if (target != null && !target.Data.IsDead)
                {
                    Arrow arrow = new(Palette.CrewmateBlue);
                    arrow.arrow.SetActive(true);
                    arrow.Update(target.transform.position);
                    arrows.Add(arrow);
                    if (targetPositionText == null)
                    {
                        RoomTracker roomTracker = HudManager.Instance?.roomTracker;
                        if (roomTracker == null) return;
                        GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
                        UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                        gameObject.transform.SetParent(HudManager.Instance.transform);
                        gameObject.transform.localPosition = new Vector3(0, -2.0f, gameObject.transform.localPosition.z);
                        gameObject.transform.localScale = Vector3.one * 1.0f;
                        targetPositionText = gameObject.GetComponent<TMPro.TMP_Text>();
                        targetPositionText.alpha = 1.0f;
                    }
                    PlainShipRoom room = Helpers.getPlainShipRoom(target);
                    targetPositionText.gameObject.SetActive(true);
                    int nearestPlayer = 0;
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (p != target && !p.Data.IsDead)
                        {
                            float dist = Vector2.Distance(p.transform.position, target.transform.position);
                            if (dist < 7f) nearestPlayer += 1;
                        }
                    }
                    if (room != null)
                    {
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}({nearestPlayer})(" + DestroyableSingleton<TranslationController>.Instance.GetString(room.RoomId) + ")</color>";
                    }
                    else
                    {
                        targetPositionText.text = "<color=#8CFFFFFF>" + $"{target.Data.PlayerName}({nearestPlayer})</color>";
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
            if (PlayerControl.LocalPlayer != moriarty) return;
            if (arrows.Count > 0)
            {
                foreach (var arrow in arrows)
                    if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            }
            if (targetPositionText != null) targetPositionText.gameObject.SetActive(false);
            var obj = GameObject.Find("MoriartyText(Clone)");
            if (obj != null) obj.SetActive(false);
        }

        public static void generateBrainwashText()
        {
            TMPro.TMP_Text text;
            RoomTracker roomTracker = HudManager.Instance?.roomTracker;
            GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
            gameObject.transform.SetParent(HudManager.Instance.transform);
            gameObject.transform.localPosition = new Vector3(0, -1.3f, gameObject.transform.localPosition.z);
            gameObject.transform.localScale = Vector3.one * 3f;
            text = gameObject.GetComponent<TMPro.TMP_Text>();
            text.name = "MoriartyText(Clone)";
            PlayerControl tmpP = target;
            bool done = false;
            HudManager.Instance.StartCoroutine(Effects.Lerp(brainwashTime, new Action<float>((p) =>
            {
                if (done)
                {
                    return;
                }
                if (target == null || MeetingHud.Instance != null || p == 1f)
                {
                    if (text != null && text.gameObject) UnityEngine.Object.Destroy(text.gameObject);
                    if (target == tmpP) target = null;
                    done = true;
                    return;
                }
                else
                {
                    string message = (brainwashTime - (p * brainwashTime)).ToString("0");
                    bool even = ((int)(p * brainwashTime / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds
                                                                                      // string prefix = even ? "<color=#555555FF>" : "<color=#FFFFFFFF>";
                    string prefix = "<color=#555555FF>";
                    text.text = prefix + message + "</color>";
                    if (text != null) text.color = even ? Color.yellow : Color.red;
                }
            })));
        }

        public static void clearAndReload()
        {
            moriarty = null;
            formerMoriarty = null;
            tmpTarget = null;
            target = null;
            currentTarget = null;
            killTarget = null;
            brainwashed = new List<PlayerControl>();
            counter = 0;
            triggerMoriartyWin = false;
            hasKilled = false;
            brainwashCooldown = CustomOptionHolder.moriartyBrainwashCooldown.getFloat();
            brainwashTime = CustomOptionHolder.moriartyBrainwashTime.getFloat();
            numberToWin = (int)CustomOptionHolder.moriartyNumberToWin.getFloat();
            indicateKills = CustomOptionHolder.moriartyKillIndicate.getBool();

            if (targetPositionText != null) UnityEngine.Object.Destroy(targetPositionText);
            targetPositionText = null;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();

            var obj = GameObject.Find("MoriartyText(Clone)");
            if (obj != null) UnityEngine.Object.Destroy(obj);
        }
    }
}
    