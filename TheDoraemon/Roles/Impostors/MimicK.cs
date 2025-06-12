using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class MimicK
    {
        public static PlayerControl mimicK;
        public static Color color = Palette.ImpostorRed;

        public static bool ifOneDiesBothDie = true;
        public static bool hasOneVote = true;
        public static bool countAsOne = true;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;

        public static AchievementToken<int> acTokenChallenge;


        public static PlayerControl victim;

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer != mimicK || PlayerControl.LocalPlayer.Data.IsDead || MimicA.mimicA == null)
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
                // Arrows一覧
                arrows = new List<Arrow>();

                if (MimicA.mimicA.Data.IsDead || MimicA.mimicA == null) return;
                Arrow arrow;
                arrow = MimicA.isMorph ? new Arrow(Palette.White) : new Arrow(Palette.ImpostorRed);
                arrow.arrow.SetActive(true);
                arrow.Update(MimicA.mimicA.transform.position);
                arrows.Add(arrow);

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
        }

        public static void onAchievementActivate()
        {
            if (mimicK == null || PlayerControl.LocalPlayer != mimicK) return;
            acTokenChallenge ??= new("mimicK.challenge", 0, (val, _) => val >= 3);
        }

        public static void clearAndReload()
        {
            if (mimicK != null && mimicK?.Data != null) mimicK.setDefaultLook();
            if (MimicA.mimicA != null)
            {
                MimicA.isMorph = false;
                MimicA.mimicA.setDefaultLook();
            }

            mimicK = null;
            victim = null;
            ifOneDiesBothDie = CustomOptionHolder.mimicIfOneDiesBothDie.getBool();
            hasOneVote = CustomOptionHolder.mimicHasOneVote.getBool();
            countAsOne = CustomOptionHolder.mimicCountAsOne.getBool();

            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            acTokenChallenge = null;
        }
    }
}
    