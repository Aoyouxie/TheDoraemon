using Epic.OnlineServices.RTCAudio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheDoraemon.Roles;
using static TheDoraemon.CustomOption;
using static TheDoraemon.Roles.TimeSpaceManager;
using Types = TheDoraemon.CustomOption.CustomOptionType;
using Il2CppSystem.Buffers;


namespace TheDoraemon
{
    public class CustomOptionHolder
    {
        public static string[] rates = new string[] { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };
        public static string[] ratesModifier = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        public static string[] presets = new string[] { "preset1", "preset2", "Random Preset Skeld", "Random Preset Mira HQ", "Random Preset Polus", "Random Preset Airship", "Random Preset Submerged" };

        public static CustomOption presetSelection;
        public static CustomOption activateRoles;
        public static CustomOption crewmateRolesCountMin;
        public static CustomOption crewmateRolesCountMax;
        public static CustomOption crewmateRolesFill;
        public static CustomOption neutralRolesCountMin;
        public static CustomOption neutralRolesCountMax;
        public static CustomOption impostorRolesCountMin;
        public static CustomOption impostorRolesCountMax;
        public static CustomOption modifiersCountMin;
        public static CustomOption modifiersCountMax;

        public static CustomOption timeSpaceManagerSpawnRate;
        public static CustomOption timeSpaceManagerCooldown;
        public static CustomOption timeSpaceManagerCanKillNeutrals;

        internal static Dictionary<byte, byte[]> blockedRolePairings = new();

        public static string cs(Color c, string s)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        //public static bool isMapSelectionOption(CustomOption option)
        //{
        //    return option == hideNSeekMap;
        //}

        public static void Load()
        {

            CustomOption.vanillaSettings = TheDoraemonPlugin.Instance.Config.Bind("Preset0", "VanillaOptions", "");

            // Role Options
            presetSelection = CustomOption.Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "presetSelection"), presets, null, true);

            timeSpaceManagerSpawnRate = CustomOption.Create(1, Types.Crewmate, cs(TimeSpaceManager.TimeSpaceManagerColor, "timeSpaceManager"), rates, null, true);
            timeSpaceManagerCooldown = CustomOption.Create(2, Types.Crewmate, "timeSpaceManagerCooldown", 30f, 10f, 60f, 2.5f, timeSpaceManagerSpawnRate, false, "unitSeconds");
            timeSpaceManagerCanKillNeutrals = CustomOption.Create(3, Types.Crewmate, "timeSpaceManagerCanKillNeutrals", false, timeSpaceManagerSpawnRate);
        }
    }
}
