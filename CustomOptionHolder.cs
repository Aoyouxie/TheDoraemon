using Epic.OnlineServices.RTCAudio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheDoraemon.Roles;
using Types = TheDoraemon.CustomOption.CustomOptionType;

namespace TheDoraemon
{
   public class CustomOptionHolder
    {
        public static string[] rates = new string[] { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };
        public static string[] ratesModifier = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
        public static string[] presets = new string[] { "preset1", "preset2", "Random Preset Skeld", "Random Preset Mira HQ", "Random Preset Polus", "Random Preset Airship", "Random Preset Submerged" };

        public static CustomOption sheriffSpawnRate;
        public static CustomOption sheriffCooldown;
        public static CustomOption sheriffCanKillNeutrals;
        internal static object timeSpaceManagerCooldown;
        //  public static CustomOption deputySpawnRate;

    }
}
