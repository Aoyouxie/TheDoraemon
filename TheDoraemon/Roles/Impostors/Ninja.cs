using System.Linq;
using HarmonyLib;
using System;
using UnityEngine;

using TheDoraemon.Modules;
using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Modifier;





namespace TheDoraemon.Roles.Impostors
{
    public static class Ninja
    {
        public static PlayerControl ninja;
        public static Color color = Palette.ImpostorRed;
        public static float stealthCooldown = 30f;
        public static float stealthDuration = 15f;
        public static float killPenalty = 10f;
        public static float speedBonus = 1.25f;
        public static float fadeTime = 0.5f;
        public static bool canUseVents = false;
        public static bool canBeTargeted;

        public static bool penalized = false;
        public static bool stealthed = false;
        public static DateTime stealthedAt = DateTime.UtcNow;
        public static AchievementToken<int> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (ninja == null || PlayerControl.LocalPlayer != ninja) return;
            acTokenChallenge ??= new("ninja.challenge", 0, (val, _) => val >= 2);

        }

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.NinjaButton.png", 115f);
            return buttonSprite;
        }

        public static bool isStealthed(PlayerControl player)
        {
            if (ninja != null && !ninja.Data.IsDead && ninja == player)
            {
                return stealthed;
            }
            return false;
        }

        public static float stealthFade(PlayerControl player)
        {
            if (ninja == player && fadeTime > 0 && !ninja.Data.IsDead)
            {
                return Mathf.Min(1.0f, (float)(DateTime.UtcNow - stealthedAt).TotalSeconds / fadeTime);
            }
            return 1.0f;
        }

        public static void setStealthed(PlayerControl player, bool stealthed = true)
        {
            if (ninja == player && ninja != null)
            {
                Ninja.stealthed = stealthed;
                stealthedAt = DateTime.UtcNow;
            }
        }

        public static void setOpacity(PlayerControl player, float opacity)
        {
            var color = Color.Lerp(Palette.ClearWhite, Palette.White, opacity);
            try
            {
                // Block setting opacity if the Chameleon skill is active
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId) && Chameleon.visibility(player.PlayerId) < 1f && !stealthed) return;
                Helpers.setInvisible(player, color, opacity);
            }
            catch { }
        }

        public static void clearAndReload()
        {
            setOpacity(ninja, 1.0f);
            ninja = null;
            stealthCooldown = CustomOptionHolder.ninjaStealthCooldown.getFloat();
            stealthDuration = CustomOptionHolder.ninjaStealthDuration.getFloat();
            killPenalty = CustomOptionHolder.ninjaKillPenalty.getFloat();
            speedBonus = CustomOptionHolder.ninjaSpeedBonus.getFloat();
            fadeTime = CustomOptionHolder.ninjaFadeTime.getFloat();
            canUseVents = CustomOptionHolder.ninjaCanVent.getBool();
            canBeTargeted = CustomOptionHolder.ninjaCanBeTargeted.getBool();

            penalized = false;
            stealthed = false;
            stealthedAt = DateTime.UtcNow;

            acTokenChallenge = null;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysicsNinjaPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.AmOwner && __instance.myPlayer.CanMove && GameData.Instance && isStealthed(__instance.myPlayer))
                {
                    __instance.body.velocity *= speedBonus + 1;
                }

                if (__instance.myPlayer == ninja)
                {
                    var ninja = __instance.myPlayer;
                    if (ninja == null || ninja.Data.IsDead) return;

                    bool canSee =
                        PlayerControl.LocalPlayer.Data.IsDead && !
                        PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                        Lighter.canSeeInvisible && PlayerControl.LocalPlayer == Lighter.lighter;

                    var opacity = canSee ? 0.5f : 0.0f;

                    if (isStealthed(ninja))
                    {
                        opacity = Math.Max(opacity, 1.0f - stealthFade(ninja));
                        ninja.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                    }
                    else
                    {
                        opacity = Math.Max(opacity, stealthFade(ninja));
                    }

                    setOpacity(ninja, opacity);
                }
            }
        }
    }
}
