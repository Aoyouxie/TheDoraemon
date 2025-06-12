using System.Linq;
using System;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Modifier;





namespace TheDoraemon.Roles.Neutrals
{
    public static class Kataomoi
    {
        public static PlayerControl kataomoi;
        public static Color color = Lovers.color;

        public static float stareCooldown = 30f;
        public static float stareDuration = 3f;
        public static int stareCount = 1;
        public static int stareCountMax = 1;
        public static float stalkingCooldown = 30f;
        public static float stalkingDuration = 5f;
        public static float stalkingFadeTime = 0.5f;
        public static float searchCooldown = 30f;
        public static float searchDuration = 5f;
        public static bool isSearch = false;
        public static float stalkingTimer = 0f;
        public static float stalkingEffectTimer = 0f;
        public static bool triggerKataomoiWin = false;
        public static PlayerControl target = null;
        public static PlayerControl currentTarget = null;
        public static TMPro.TextMeshPro stareText = null;
        public static SpriteRenderer[] gaugeRenderer = new SpriteRenderer[3];
        public static Arrow arrow;
        public static float gaugeTimer = 0.0f;
        public static float baseGauge = 0f;

        static bool _isStalking = false;

        static Sprite stareSprite;
        public static Sprite getStareSprite()
        {
            if (stareSprite) return stareSprite;
            stareSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.KataomoiStareButton.png", 115f);
            return stareSprite;
        }

        static Sprite loveSprite;
        public static Sprite getLoveSprite()
        {
            if (loveSprite) return loveSprite;
            loveSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.KataomoiLoveButton.png", 115f);
            return loveSprite;
        }

        static Sprite searchSprite;
        public static Sprite getSearchSprite()
        {
            if (searchSprite) return searchSprite;
            searchSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.KataomoiSearchButton.png", 115f);
            return searchSprite;
        }

        static Sprite stalkingSprite;
        public static Sprite getStalkingSprite()
        {
            if (stalkingSprite) return stalkingSprite;
            stalkingSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.KataomoiStalkingButton.png", 115f);
            return stalkingSprite;
        }

        static Sprite[] loveGaugeSprites = new Sprite[3];
        public static Sprite getLoveGaugeSprite(int index)
        {
            if (index < 0 || index >= loveGaugeSprites.Length) return null;
            if (loveGaugeSprites[index]) return loveGaugeSprites[index];

            int id = 0;
            switch (index)
            {
                case 0: id = 1; break;
                case 1: id = 2; break;
                case 2: id = 11; break;
            }
            loveGaugeSprites[index] = Helpers.loadSpriteFromResources(String.Format("TheDoraemon.Resources.KataomoiGauge_{0:d2}.png", id), 115f);
            return loveGaugeSprites[index];
        }

        public static void doStare()
        {
            baseGauge = getLoveGauge();
            gaugeTimer = 1.0f;
            stareCount = Mathf.Max(0, stareCount - 1);

            if (gaugeRenderer[2] != null && stareCount == 0)
            {
                gaugeRenderer[2].color = color;
            }
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
        }

        public static void doStalking()
        {
            if (kataomoi == null) return;
            stalkingTimer = stalkingDuration;
            _isStalking = true;
        }

        public static void resetStalking()
        {
            if (kataomoi == null) return;
            _isStalking = false;
            setAlpha(1.0f);
        }

        public static bool isStalking(PlayerControl player)
        {
            if (player == null || player != kataomoi) return false;
            return _isStalking && stalkingTimer > 0;
        }

        public static bool isStalking()
        {
            return isStalking(kataomoi);
        }

        public static void doSearch()
        {
            if (kataomoi == null) return;
            isSearch = true;
        }

        public static void resetSearch()
        {
            if (kataomoi == null) return;
            isSearch = false;
        }

        public static bool canLove()
        {
            return stareCount <= 0;
        }

        public static float getLoveGauge()
        {
            return 1.0f - (stareCountMax == 0 ? 0f : (float)stareCount / (float)stareCountMax);
        }

        public static void resetAllArrow()
        {
            if (PlayerControl.LocalPlayer != kataomoi) return;
            TORMapOptions.resetPoolables();
            for (int i = 0; i < gaugeRenderer.Length; ++i)
            {
                if (gaugeRenderer[i] != null)
                {
                    gaugeRenderer[i].gameObject.SetActive(false);
                }
            }
            if (stareText != null) stareText.gameObject.SetActive(false);
        }

        public static void clearAndReload()
        {
            resetStalking();

            kataomoi = null;
            stareCooldown = CustomOptionHolder.kataomoiStareCooldown.getFloat();
            stareDuration = CustomOptionHolder.kataomoiStareDuration.getFloat();
            stareCount = stareCountMax = (int)CustomOptionHolder.kataomoiStareCount.getFloat();
            stalkingCooldown = CustomOptionHolder.kataomoiStalkingCooldown.getFloat();
            stalkingDuration = CustomOptionHolder.kataomoiStalkingDuration.getFloat();
            stalkingFadeTime = CustomOptionHolder.kataomoiStalkingFadeTime.getFloat();
            searchCooldown = CustomOptionHolder.kataomoiSearchCooldown.getFloat();
            searchDuration = CustomOptionHolder.kataomoiSearchDuration.getFloat();
            isSearch = false;
            stalkingTimer = 0f;
            stalkingEffectTimer = 0f;
            triggerKataomoiWin = false;
            target = null;
            currentTarget = null;
            TORMapOptions.resetPoolables();
            if (stareText != null) UnityEngine.Object.Destroy(stareText);
            stareText = null;
            if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            for (int i = 0; i < gaugeRenderer.Length; ++i)
            {
                if (gaugeRenderer[i] != null)
                {
                    UnityEngine.Object.Destroy(gaugeRenderer[i].gameObject);
                    gaugeRenderer[i] = null;
                }
            }
            arrow = null;
            gaugeTimer = 0.0f;
            baseGauge = 0.0f;
        }

        public static void fixedUpdate(PlayerPhysics __instance)
        {
            if (kataomoi == null) return;
            if (kataomoi != __instance.myPlayer) return;

            if (gaugeRenderer[1] != null && gaugeTimer > 0.0f)
            {
                gaugeTimer = Mathf.Max(gaugeTimer - Time.fixedDeltaTime, 0.0f);
                float gauge = getLoveGauge();
                float nowGauge = Mathf.Lerp(baseGauge, gauge, 1.0f - gaugeTimer);
                gaugeRenderer[1].transform.localPosition = new Vector3(Mathf.Lerp(-3.470784f - 1.121919f + 1.25f, -3.470784f + 1.25f, nowGauge), -2.626999f, -8.1f);
                gaugeRenderer[1].transform.localScale = new Vector3(nowGauge, 1, 1);
            }

            if (kataomoi.Data.IsDead) return;
            if (_isStalking && stalkingTimer > 0)
            {
                kataomoi.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                stalkingTimer = Mathf.Max(0f, stalkingTimer - Time.fixedDeltaTime);
                if (stalkingFadeTime > 0)
                {
                    float elapsedTime = stalkingDuration - stalkingTimer;
                    float alpha = Mathf.Min(elapsedTime, stalkingFadeTime) / stalkingFadeTime;
                    alpha = Mathf.Clamp(1f - alpha, PlayerControl.LocalPlayer == kataomoi || PlayerControl.LocalPlayer.Data.IsDead
                        || (PlayerControl.LocalPlayer == Lighter.lighter && Lighter.canSeeInvisible) ? 0.5f : 0f, 1f);
                    setAlpha(alpha);
                }
                else
                {
                    setAlpha(PlayerControl.LocalPlayer == kataomoi ? 0.5f : 0f);
                }

                if (stalkingTimer <= 0f)
                {
                    _isStalking = false;
                    stalkingEffectTimer = stalkingFadeTime;
                }
            }
            else if (!_isStalking && stalkingEffectTimer > 0)
            {
                stalkingEffectTimer = Mathf.Max(0f, stalkingEffectTimer - Time.fixedDeltaTime);
                if (stalkingFadeTime > 0)
                {
                    float elapsedTime = stalkingFadeTime - stalkingEffectTimer;
                    float alpha = Mathf.Min(elapsedTime, stalkingFadeTime) / stalkingFadeTime;
                    alpha = Mathf.Clamp(alpha, PlayerControl.LocalPlayer == kataomoi || PlayerControl.LocalPlayer.Data.IsDead ? 0.5f : 0f, 1f);
                    setAlpha(alpha);
                }
                else
                {
                    setAlpha(1.0f);
                }
            }
            else
            {
                setAlpha(1.0f);
            }
        }

        static void setAlpha(float alpha)
        {
            if (kataomoi == null) return;
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, alpha);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == kataomoi.PlayerId) && Chameleon.visibility(kataomoi.PlayerId) < 1f && !isStalking()) return;
                Helpers.setInvisible(kataomoi, color, alpha);
            }
            catch { }
        }
    }
}
