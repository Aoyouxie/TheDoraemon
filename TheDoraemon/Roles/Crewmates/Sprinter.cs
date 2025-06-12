using System.Linq;
using HarmonyLib;
using System;
using UnityEngine;
using TheDoraemon.Modules;
using TheDoraemon.Roles.Modifier;

namespace TheDoraemon.Roles.Crewmates
{
    public static class Sprinter
    {
        public static PlayerControl sprinter;
        public static Color color = new Color32(128, 128, 255, byte.MaxValue);

        public static float sprintCooldown = 30f;
        public static float sprintDuration = 15f;
        public static float fadeTime = 0.5f;
        public static float speedBonus = 0.25f;

        public static bool sprinting = false;

        public static DateTime sprintAt = DateTime.UtcNow;

        public static AchievementToken<(Vector3 pos, bool cleared)> acTokenMove = null;

        public static void onAchievementActivate()
        {
            if (sprinter == null || PlayerControl.LocalPlayer != sprinter) return;
            if (sprintDuration <= 15f)
                acTokenMove ??= new("sprinter.common2", (Vector3.zero, false), (val, _) => val.cleared);
        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("namespace TheDoraemon.Resources.SprintButton.png", 115f);
            return buttonSprite;
        }

        public static float sprintFade(PlayerControl player)
        {
            if (sprinter == player && fadeTime > 0 && !sprinter.Data.IsDead)
            {
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - sprintAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static void setSprinting(PlayerControl player, bool sprinting = true)
        {
            if (sprinter != null && player == sprinter)
            {
                Sprinter.sprinting = sprinting;
                sprintAt = DateTime.UtcNow;
            }
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !sprinting) return;
                Helpers.setInvisible(player, color, opacity);
            }
            catch { }
        }

        public static void clearAndReload()
        {
            setOpacity(sprinter, 1.0f);
            sprinter = null;
            sprinting = false;
            sprintCooldown = CustomOptionHolder.sprinterCooldown.getFloat();
            sprintDuration = CustomOptionHolder.sprinterDuration.getFloat();
            fadeTime = CustomOptionHolder.sprinterFadeTime.getFloat();
            speedBonus = CustomOptionHolder.sprinterSpeedBonus.getFloat();
            acTokenMove = null;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsSprinterPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.myPlayer == sprinter)
                {
                    if (GameData.Instance && sprinting && __instance.AmOwner && __instance.myPlayer.CanMove) __instance.body.velocity *= 1 + speedBonus;
                    var sprinter = __instance.myPlayer;
                    if (sprinter == null || sprinter.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.Data.IsDead && !
                        PlayerControl.LocalPlayer == Sprinter.sprinter ||
                        Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter;

                    var opacity = canSee ? 0.5f : 0.0f;

                    if (sprinting)
                    {
                        opacity = Math.Max(opacity, 1.0f - sprintFade(sprinter));
                        sprinter.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, sprintFade(sprinter));
                    }

                    setOpacity(sprinter, opacity);
                }
            }
        }
    }
}
