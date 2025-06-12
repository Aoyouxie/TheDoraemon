using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Utilities;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class MimicA
    {
        public static PlayerControl mimicA;
        public static Color color = Palette.ImpostorRed;

        public static bool isMorph = false;

        //public static string MimicKName = MimicK.mimicK.Data.PlayerName;

        public static Sprite adminButtonSprite;
        public static Sprite morphButtonSprite;

        public static Sprite getMorphSprite()
        {
            if (morphButtonSprite) return morphButtonSprite;
            morphButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.MorphButton.png", 115f);
            return morphButtonSprite;
        }

        public static Sprite getAdminSprite()
        {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            adminButtonSprite = button.Image;
            return adminButtonSprite;
        }

        public static AchievementToken<int> acTokenCommon;

        public static void onAchievementActivate()
        {
            if (mimicA == null || PlayerControl.LocalPlayer != mimicA) return;
            acTokenCommon ??= new("mimicA.challenge", 0, (val, _) => val >= 4);
        }

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static void arrowUpdate()
        {
            if (MimicK.mimicK == null || MimicK.mimicK.Data.IsDead || MimicK.mimicK.Data.Disconnected) {
                if (mimicA != null && isMorph) {
                    isMorph = false;
                    mimicA.setDefaultLook();
                }
            }

            if (PlayerControl.LocalPlayer != mimicA || PlayerControl.LocalPlayer.Data.IsDead || MimicK.mimicK == null)
            {
                if (arrows.FirstOrDefault()?.arrow != null)
                    foreach (Arrow arrows in arrows) arrows.arrow.SetActive(false);
                return;
            }

            // 前フレームからの経過時間をマイナスする
            updateTimer -= Time.fixedDeltaTime;

            // 1秒経過したらArrowを更新
            if (updateTimer <= 0.0f)
            {

                // 前回のArrowをすべて破棄する
                foreach (Arrow arrow1 in arrows)
                {
                    if (arrow1 != null && arrow1.arrow != null)
                    {
                        arrow1.arrow.SetActive(false);
                        Object.Destroy(arrow1.arrow);
                    }
                }

                //if (MimicA.mimicA == null) return;

                // Arrows一覧
                arrows = new List<Arrow>();
                if (MimicK.mimicK.Data.IsDead || MimicK.mimicK == null) return;
                Arrow arrow = new(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicK.mimicK.transform.position);
                arrows.Add(arrow);

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void clearAndReload()
        {
            if (mimicA != null && mimicA?.Data != null) mimicA.setDefaultLook();
            mimicA = null;
            isMorph = false;
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            acTokenCommon = null;
        }
    }
}
    