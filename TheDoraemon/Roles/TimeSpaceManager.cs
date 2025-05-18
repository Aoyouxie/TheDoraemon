using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
//using TheDoraemon.Objects;
//using TheDoraemon.Utilities;
//using TheDoraemon.CustomGameModes;
//using static TheDoraemonRoles;
//using AmongUs.Data;
//using Hazel;
//using TheDoraemon.Patches;
//using Reactor.Utilities.Extensions;
//using TheDoraemon.Modules;
//using AmongUs.GameOptions;
//using UnityEngine.Events;
//using UnityEngine.UI;
//using System.Collections;
//using Reactor.Utilities;
//using TheDoraemon.MetaContext;
//using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;
//using static UnityEngine.ParticleSystem.PlaybackState;
//using UnityEngine.UIElements;



namespace TheDoraemon.Roles
{

    public static class TimeSpaceManager
    {
        public static PlayerControl timeSpaceManager;
        public static TimeSpaceManagerColor color = new Color32(248, 205, 70, byte.MaxValue);

        public static float cooldown = 30f;
        public static bool canKillNeutrals = false;
        public static bool spyCanDieToTimeSpaceManager = false;

        public static PlayerControl currentTarget;
        public static void clearAndReload()
        {
            timeSpaceManager = null;
            currentTarget = null;
            cooldown = CustomOptionHolder.timeSpaceManagerCooldown.getFloat();
            canKillNeutrals = CustomOptionHolder.timeSpaceManagerCanKillNeutrals.getBool();
           
        }
    }

}

  

public class TimeSpaceManagerColor
{
}