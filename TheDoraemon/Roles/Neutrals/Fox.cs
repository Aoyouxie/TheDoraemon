using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TheDoraemon.Objects;
using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Modifier;


namespace TheDoraemon.Roles.Neutrals
{
    public static class Fox
    {
        public static PlayerControl fox;
        public static Color color = new Color32(167, 87, 168, byte.MaxValue);

        public enum TaskType
        {
            Serial,
            Parallel
        }

        public static List<Arrow> arrows = new();
        public static float updateTimer = 0f;
        public static float arrowUpdateInterval = 0.5f;
        public static bool crewWinsByTasks = false;
        public static bool impostorWinsBySabotage = true;
        public static float stealthCooldown;
        public static float stealthDuration;
        public static int numTasks;
        public static float stayTime;

        public static bool stealthed = false;
        public static DateTime stealthedAt = DateTime.UtcNow;
        public static float fadeTime = 1f;

        public static int numRepair = 0;
        public static bool canCreateImmoralist;
        public static PlayerControl currentTarget;
        public static TaskType taskType;

        private static Sprite hideButtonSprite;
        private static Sprite repairButtonSprite;
        private static Sprite immoralistButtonSprite;

        public static Sprite getHideButtonSprite()
        {
            if (hideButtonSprite) return hideButtonSprite;
            hideButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.FoxHideButton.png", 115f);
            return hideButtonSprite;
        }

        public static Sprite getRepairButtonSprite()
        {
            if (repairButtonSprite) return repairButtonSprite;
            repairButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.RepairButton.png", 115f);
            return repairButtonSprite;
        }

        public static Sprite getImmoralistButtonSprite()
        {
            if (immoralistButtonSprite) return immoralistButtonSprite;
            immoralistButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.FoxImmoralistButton.png", 115f);
            return immoralistButtonSprite;
        }

        public static float stealthFade()
        {
            if (fox != null && !fox.Data.IsDead)
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - stealthedAt).TotalSeconds / fadeTime);
            return 1.0f;
        }

        public static void setStealthed(bool stealthed = true)
        {
            Fox.stealthed = stealthed;
            stealthedAt = DateTime.UtcNow;
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
                Helpers.setInvisible(player, color, opacity);
            }
            catch { }
        }

        public static bool tasksComplete()
        {
            if (fox == null) return false;
            if (fox.Data.IsDead) return false;
            int counter = 0;
            int totalTasks = 1;
            foreach (var task in fox.Data.Tasks)
            {
                if (task.Complete)
                {
                    counter++;
                }
            }
            return counter == totalTasks;
        }

        public static void clearAllArrow()
        {
            if (PlayerControl.LocalPlayer != fox) return;
            if (arrows?.Count > 0) {
                foreach (var arrow in arrows)
                    if (arrow != null && arrow.arrow != null) arrow.arrow.SetActive(false);
            }
        }

        public static void arrowUpdate()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (arrows?.Count > 0) {
                    foreach (var arrow in arrows)
                        if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
                }
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
                    if (arrow?.arrow != null)
                    {
                        arrow.arrow.SetActive(false);
                        UnityEngine.Object.Destroy(arrow.arrow);
                    }
                }

                // Arrows一覧
                arrows = new List<Arrow>();

                // インポスターの位置を示すArrowsを描画
                foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) continue;
                    Arrow arrow;
                    // float distance = Vector2.Distance(p.transform.position, PlayerControl.LocalPlayer.transform.position);
                    if (p.Data.Role.IsImpostor || p == Jackal.jackal || p ==TimespaceManager.timespaceManager || p == JekyllAndHyde.jekyllAndHyde ||
                        p == Moriarty.moriarty || p == Thief.thief || p == SchrodingersCat.schrodingersCat && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate ||
                        p == Sidekick.sidekick && Sidekick.canKill)
                    {
                        if (p.Data.Role.IsImpostor)
                        {
                            arrow = new Arrow(Palette.ImpostorRed);
                        }
                        else if (p == Jackal.jackal || p == Sidekick.sidekick)
                        {
                            arrow = new Arrow(Jackal.color);
                        }
                        else if (p == TimespaceManager.timespaceManager)
                        {
                            arrow = new Arrow(Palette.White);
                        }
                        else if (p == JekyllAndHyde.jekyllAndHyde)
                        {
                            arrow = new Arrow(JekyllAndHyde.color);
                        }
                        else if (p == Moriarty.moriarty)
                        {
                            arrow = new Arrow(Moriarty.color);
                        }
                        else if (p == SchrodingersCat.schrodingersCat)
                        {
                            arrow = new Arrow(RoleInfo.schrodingersCat.color);
                        }
                        else
                        {
                            arrow = new Arrow(Thief.color);
                        }
                        arrow.arrow.SetActive(true);
                        arrow.Update(p.transform.position);
                        arrows.Add(arrow);
                    }
                }

                // タイマーに時間をセット
                updateTimer = arrowUpdateInterval;
            }
            else
            {
                arrows.Do(x => x.Update());
            }
        }

        public static void clearAndReload()
        {
            setOpacity(fox, 1.0f);
            fox = null;
            currentTarget = null;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;
            crewWinsByTasks = CustomOptionHolder.foxCrewWinsByTasks.getBool();
            impostorWinsBySabotage = CustomOptionHolder.foxImpostorWinsBySabotage.getBool();
            stealthCooldown = CustomOptionHolder.foxStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.foxStealthDuration.getFloat();
            canCreateImmoralist = CustomOptionHolder.foxCanCreateImmoralist.getBool();
            numTasks = (int)CustomOptionHolder.foxNumTasks.getFloat();
            numRepair = (int)CustomOptionHolder.foxNumRepairs.getFloat();
            stayTime = (int)CustomOptionHolder.foxStayTime.getFloat();
            taskType = (TaskType)CustomOptionHolder.foxTaskType.getSelection();
            foreach (Arrow arrow in arrows)
            {
                if (arrow?.arrow != null)
                {
                    arrow.arrow.SetActive(false);
                    UnityEngine.Object.Destroy(arrow.arrow);
                }
            }
            arrows = new List<Arrow>();
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsFoxPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (fox != null && fox == __instance.myPlayer)
                {
                    var fox = __instance.myPlayer;
                    if (fox == null || fox.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer == fox ||
                        PlayerControl.LocalPlayer.Data.IsDead && !
                        PlayerControl.LocalPlayer == Lighter.lighter && Lighter.canSeeInvisible ||
                        PlayerControl.LocalPlayer == Immoralist.immoralist;

                    var opacity = canSee ? 0.5f : 0.0f;

                    if (stealthed)
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade());
                        fox.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade());
                    }

                    setOpacity(fox, opacity);
                }
            }
        }
    }
}
