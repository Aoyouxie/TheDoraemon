using System.Collections.Generic;
using UnityEngine;
using AmongUs.Data;
using TheDoraemon.Roles.Crewmates;
using TheDoraemon.Roles.Impostors;
using TheDoraemon.Roles.Neutrals;


namespace TheDoraemon.Roles.Modifier
{
    public static class Chameleon
    {
        public static List<PlayerControl> chameleon = new();
        public static float minVisibility = 0.2f;
        public static float holdDuration = 1f;
        public static float fadeDuration = 0.5f;
        public static Dictionary<byte, float> lastMoved;

        public static void clearAndReload()
        {
            chameleon = new List<PlayerControl>();
            lastMoved = new Dictionary<byte, float>();
            holdDuration = CustomOptionHolder.modifierChameleonHoldDuration.getFloat();
            fadeDuration = CustomOptionHolder.modifierChameleonFadeDuration.getFloat();
            minVisibility = CustomOptionHolder.modifierChameleonMinVisibility.getSelection() / 10f;
        }

        public static float visibility(byte playerId)
        {
            if (playerId == Ninja.ninja?.PlayerId && Ninja.stealthed || playerId == Sprinter.sprinter?.PlayerId && Sprinter.sprinting
                || playerId == Fox.fox?.PlayerId && Fox.stealthed || playerId == Kataomoi.kataomoi?.PlayerId && Kataomoi.isStalking()) return 1f;
            float visibility = 1f;
            if (lastMoved != null && lastMoved.ContainsKey(playerId))
            {
                var tStill = Time.time - lastMoved[playerId];
                if (tStill > holdDuration)
                {
                    if (tStill - holdDuration > fadeDuration) visibility = minVisibility;
                    else visibility = (1 - (tStill - holdDuration) / fadeDuration) * (1 - minVisibility) + minVisibility;
                }
            }
            if (PlayerControl.LocalPlayer.Data.IsDead && visibility < 0.1f)
            {  // Ghosts can always see!
                visibility = 0.1f;
            }
            return visibility;
        }

        public static void update()
        {
            foreach (var chameleonPlayer in chameleon)
            {
                //if (chameleonPlayer == Assassin.assassin && Assassin.isInvisble) continue;  // Dont make Assassin visible...
                if (chameleonPlayer == Ninja.ninja && Ninja.stealthed || chameleonPlayer == Sprinter.sprinter && Sprinter.sprinting || chameleonPlayer == Fox.fox && Fox.stealthed ||
                    chameleonPlayer == Kataomoi.kataomoi && Kataomoi.isStalking()) continue;
                // check movement by animation
                PlayerPhysics playerPhysics = chameleonPlayer.MyPhysics;
                var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
                if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim)
                {
                    lastMoved[chameleonPlayer.PlayerId] = Time.time;
                }
                // calculate and set visibility
                float visibility = Chameleon.visibility(chameleonPlayer.PlayerId);
                float petVisibility = visibility;
                if (chameleonPlayer.Data.IsDead)
                {
                    visibility = 0.5f;
                    petVisibility = 1f;
                }

                try
                {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                    chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color = chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
                    if (DataManager.Settings.Accessibility.ColorBlindMode) chameleonPlayer.cosmetics.colorBlindText.color = chameleonPlayer.cosmetics.colorBlindText.color.SetAlpha(visibility);
                    chameleonPlayer.SetHatAndVisorAlpha(visibility);
                    chameleonPlayer.cosmetics.skin.layer.color = chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility);
                    chameleonPlayer.cosmetics.nameText.color = chameleonPlayer.cosmetics.nameText.color.SetAlpha(visibility);
                    foreach (var rend in chameleonPlayer.cosmetics.currentPet.renderers) rend.color = rend.color.SetAlpha(petVisibility);
                    foreach (var shadowRend in chameleonPlayer.cosmetics.currentPet.shadows) shadowRend.color = shadowRend.color.SetAlpha(petVisibility);

                    //if (chameleonPlayer.cosmetics.skin.layer.color == chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility) && visibility == minVisibility) TheDoraemonPlugin.Logger.LogMessage("Chameleon");
                    //chameleonPlayer.cosmetics.currentPet.renderers[0].color = chameleonPlayer.cosmetics.currentPet.renderers[0].color.SetAlpha(petVisibility);
                    //chameleonPlayer.cosmetics.currentPet.shadows[0].color = chameleonPlayer.cosmetics.currentPet.shadows[0].color.SetAlpha(petVisibility);
                }
                catch { }
            }

        }

        public static void removeChameleonFully(PlayerControl player)
        {
            try
            {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                player.cosmetics.currentBodySprite.BodySprite.color = player.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(1f);
                if (DataManager.Settings.Accessibility.ColorBlindMode) player.cosmetics.colorBlindText.color = player.cosmetics.colorBlindText.color.SetAlpha(1f);
                player.SetHatAndVisorAlpha(1f);
                player.cosmetics.skin.layer.color = player.cosmetics.skin.layer.color.SetAlpha(1f);
                player.cosmetics.nameText.color = player.cosmetics.nameText.color.SetAlpha(1f);
                foreach (var rend in player.cosmetics.currentPet.renderers) rend.color = rend.color.SetAlpha(1f);
                foreach (var shadowRend in player.cosmetics.currentPet.shadows) shadowRend.color = shadowRend.color.SetAlpha(1f);
                if (lastMoved.ContainsKey(player.PlayerId)) lastMoved.Remove(player.PlayerId);
            }
            catch { }
        }
    }
}
    