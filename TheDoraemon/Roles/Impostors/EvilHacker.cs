using UnityEngine;
using TheDoraemon.Utilities;
using TheDoraemon.Modules;
namespace TheDoraemon.Roles.Impostors
{
    public static class EvilHacker
    {
        public static PlayerControl evilHacker;
        public static Color color = Palette.ImpostorRed;
        public static bool canHasBetterAdmin = false;
        public static bool canCreateMadmate = false;
        public static bool canSeeDoorStatus = true;
        public static bool canCreateMadmateFromJackal;
        public static bool canInheritAbility;
        public static PlayerControl fakeMadmate;
        public static PlayerControl currentTarget;

        public static AchievementToken<(bool admin, bool cleared)> acTokenChallenge;

        public static void onAchievementActivate()
        {
            if (evilHacker == null || PlayerControl.LocalPlayer != evilHacker) return;
            acTokenChallenge ??= new("evilHacker.challenge", (false, false), (val, _) => val.cleared);
        }

        private static Sprite buttonSprite;
        private static Sprite madmateButtonSprite;

        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.isSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.isMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.isAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.isFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];
            buttonSprite = button.Image;
            return buttonSprite;
        }

        public static Sprite getMadmateButtonSprite()
        {
            if (madmateButtonSprite) return madmateButtonSprite;
            madmateButtonSprite = Helpers.loadSpriteFromResources("TheDoraemon.Resources.SidekickButton.png", 115f);
            return madmateButtonSprite;
        }

        public static bool isInherited()
        {
            return canInheritAbility && evilHacker != null && evilHacker.Data.IsDead && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
        }

        public static void clearAndReload()
        {
            evilHacker = null;
            currentTarget = null;
            fakeMadmate = null;
            canCreateMadmate = CustomOptionHolder.evilHackerCanCreateMadmate.getBool();
            canHasBetterAdmin = CustomOptionHolder.evilHackerCanHasBetterAdmin.getBool();
            canCreateMadmateFromJackal = CustomOptionHolder.evilHackerCanCreateMadmateFromJackal.getBool();
            canInheritAbility = CustomOptionHolder.evilHackerCanInheritAbility.getBool();
            canSeeDoorStatus = CustomOptionHolder.evilHackerCanSeeDoorStatus.getBool();
            acTokenChallenge = null;
        }
    }
}
    