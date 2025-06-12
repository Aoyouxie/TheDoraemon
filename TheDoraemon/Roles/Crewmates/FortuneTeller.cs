using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Utilities;
using Hazel;
using TheDoraemon.Modules;



namespace TheDoraemon.Roles.Crewmates
{
    public static class FortuneTeller
    {
        public static PlayerControl fortuneTeller;
        public static PlayerControl divineTarget;
        public static Color color = new Color32(175, 198, 241, byte.MaxValue);

        public enum DivineResults
        {
            BlackWhite,
            Team,
            Role,
        }

        public static int numTasks;
        public static DivineResults divineResult;
        public static float duration;
        public static float distance;

        public static bool endGameFlag = false;
        public static bool meetingFlag = false;

        public static Dictionary<byte, float> progress = new();
        public static Dictionary<byte, bool> playerStatus = new();
        public static bool divinedFlag = false;
        public static int numUsed = 0;

        private static Sprite leftButtonSprite;
        private static Sprite rightButtonSprite;

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;

        public static int pageIndex = 1;

        public static AchievementToken<(bool divined, bool cleared)> acTokenImpostor = null;

        public static void onAchievementActivate()
        {
            if (fortuneTeller == null || PlayerControl.LocalPlayer != fortuneTeller) return;
            acTokenImpostor ??= new("fortuneTeller.challenge", (false, false), (val, _) => val.cleared);
        }

        public static Sprite getLeftButtonSprite()
        {
            if (leftButtonSprite) return leftButtonSprite;
            leftButtonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.FortuneTellerButtonLeft.png", 130f);
            return leftButtonSprite;
        }

        public static Sprite getRightButtonSprite()
        {
            if (rightButtonSprite) return rightButtonSprite;
            rightButtonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.FortuneTellerButtonRight.png", 130f);
            return rightButtonSprite;
        }

        public static bool isCompletedNumTasks(PlayerControl p)
        {
            var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
            return tasksCompleted >= numTasks;
        }

        public static void setDivinedFlag(PlayerControl player, bool flag)
        {
            if (player == fortuneTeller)
            {
                divinedFlag = flag;
            }
        }

        public static bool canDivine(byte index)
        {
            bool status = true;
            if (playerStatus.ContainsKey(index))
            {
                status = playerStatus[index];
            }
            return progress.ContainsKey(index) && progress[index] >= duration || !status;
        }

        private static TMPro.TMP_Text text;
        public static void fortuneTellerMessage(string message, float duration, Color color)
        {
            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            if (roomTracker != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                gameObject.transform.localScale *= 1.5f;

                text = gameObject.GetComponent<TMPro.TMP_Text>();
                text.text = message;
                text.color = color;

                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
                {
                    if (p == 1f && text != null && text.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(text.gameObject);
                    }
                })));
            }
        }

        public static void divine(PlayerControl p)
        {
            string msg = "";
            Color color = Color.white;

            if (divineResult == DivineResults.BlackWhite)
            {
                if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
                {
                    msg = string.Format(ModTranslation.getString("divineMessageIsCrew"), p.Data.PlayerName);
                    color = Color.white;
                }
                else
                {
                    msg = string.Format(ModTranslation.getString("divineMessageIsntCrew"), p.Data.PlayerName);
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Team)
            {
                if (!Helpers.isNeutral(p) && !p.Data.Role.IsImpostor)
                {
                    msg = string.Format(ModTranslation.getString("divineMessageTeamCrew"), p.Data.PlayerName);
                    color = Color.white;
                }
                else if (Helpers.isNeutral(p))
                {
                    msg = string.Format(ModTranslation.getString("divineMessageTeamNeutral"), p.Data.PlayerName);
                    color = Color.yellow;
                }
                else
                {
                    msg = string.Format(ModTranslation.getString("divineMessageTeamImp"), p.Data.PlayerName);
                    color = Palette.ImpostorRed;
                }
            }

            else if (divineResult == DivineResults.Role)
            {
                msg = $"{p.Data.PlayerName} was The {string.Join(" ", RoleInfo.getRoleInfoForPlayer(p, false, true).Select(x => Helpers.cs(x.color, x.name)))}";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                fortuneTellerMessage(msg, 7f, color);
            }

            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(DestroyableSingleton<HudManager>.Instance.TaskCompleteSound, false, 0.8f);
            numUsed += 1;

            // 占いを実行したことで発火される処理を他クライアントに通知
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.FortuneTellerUsedDivine, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(p.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.fortuneTellerUsedDivine(PlayerControl.LocalPlayer.PlayerId, p.PlayerId);
        }

        public static void clearAndReload()
        {
            meetingFlag = true;
            duration = CustomOptionHolder.fortuneTellerDuration.getFloat();
            if (arrows != null)
            {
                foreach (Arrow arrow in arrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            arrows = new List<Arrow>();
            numTasks = (int)CustomOptionHolder.fortuneTellerNumTasks.getFloat();
            distance = CustomOptionHolder.fortuneTellerDistance.getFloat();
            divineResult = (DivineResults)CustomOptionHolder.fortuneTellerResults.getSelection();
            fortuneTeller = null;
            playerStatus = new Dictionary<byte, bool>();
            progress = new Dictionary<byte, float>();
            numUsed = 0;
            pageIndex = 1;
            divinedFlag = false;
            divineTarget = null;
            acTokenImpostor = null;
            TORMapOptions.resetPoolables();
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
        class IntroCutsceneOnDestroyPatch
        {
            public static void Prefix(IntroCutscene __instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(16.2f, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        meetingFlag = false;
                    }
                })));
            }
        }
    }

}
    