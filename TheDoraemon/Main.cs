global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
global using Il2CppInterop.Runtime.Injection;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using TheDoraemon.Modules;
using TheDoraemon.Utilities;
using Reactor;
using Il2CppSystem.Security.Cryptography;
using Il2CppSystem.Text;
using Reactor.Networking.Attributes;
using AmongUs.Data;
using TheDoraemon.Modules.CustomHats;
using TheDoraemon.Objects;
using TheDoraemon.MetaContext;
using UnityEngine.SceneManagement;
using AmongUs.Data.Player;

namespace TheDoraemon
{
    [BepInPlugin(Id, "TheDoraemon", VersionString)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("Among Us.exe")]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    public class TheDoraemonPlugin : BasePlugin
    {
        public const string Id = "me.eisbison.thedoraemon";
        public const string VersionString = "0.0.2.6";
        public static uint betaDays = 0;  // amount of days for the build to be usable (0 for infinite!)

        public static Version Version = Version.Parse(VersionString);
        internal static BepInEx.Logging.ManualLogSource Logger;
         
        public Harmony Harmony { get; } = new Harmony(Id);
        public static TheDoraemonPlugin Instance;

        public static int optionsPage = 2;

        public static ConfigEntry<string> DebugMode { get; private set; }
        public static ConfigEntry<bool> GhostsSeeInformation { get; set; }
        public static ConfigEntry<bool> GhostsSeeRoles { get; set; }
        public static ConfigEntry<bool> GhostsSeeModifier { get; set; }
        public static ConfigEntry<bool> GhostsSeeVotes{ get; set; }
        public static ConfigEntry<bool> ShowRoleSummary { get; set; }
        public static ConfigEntry<bool> ShowLighterDarker { get; set; }
        public static ConfigEntry<bool> EnableSoundEffects { get; set; }
        public static ConfigEntry<bool> EnableHorseMode { get; set; }
        public static ConfigEntry<string> Ip { get; set; }
        public static ConfigEntry<ushort> Port { get; set; }
        public static ConfigEntry<string> ShowPopUpVersion { get; set; }
        public static ConfigEntry<bool> ToggleCursor { get; set; }
        public static ConfigEntry<bool> ShowChatNotifications { get; set; }
        public static ConfigEntry<bool> ShowVentsOnMap { get; set; }
        public static ConfigEntry<bool> ShowExtraInfo { get; set; }

        public static Sprite ModStamp;

        public static IRegionInfo[] defaultRegions;


        // This is part of the Mini.RegionInstaller, Licensed under GPLv3
        // file="RegionInstallPlugin.cs" company="miniduikboot">
        public static void UpdateRegions() {
            ServerManager serverManager = FastDestroyableSingleton<ServerManager>.Instance;
            var regions = new IRegionInfo[] {
                new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, Ip.Value, new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Custom", Ip.Value, Port.Value, false) })).CastFast<IRegionInfo>()
            };
            
            IRegionInfo currentRegion = serverManager.CurrentRegion;
            Logger.LogInfo($"Adding {regions.Length} regions");
            foreach (IRegionInfo region in regions) {
                if (region == null) 
                    Logger.LogError("Could not add region");
                else {
                    if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase)) 
                        currentRegion = region;               
                    serverManager.AddOrUpdateRegion(region);
                }
            }

            // AU remembers the previous region that was set, so we need to restore it
            if (currentRegion != null) {
                Logger.LogDebug("Resetting previous region");
                serverManager.SetRegion(currentRegion);
            }
        }

        public override void Load() {
            ModTranslation.Load();

            Logger = Log;
            Instance = this;
  
            _ = Helpers.checkBeta(); // Exit if running an expired beta
            _ = Patches.CredentialsPatch.MOTD.loadMOTDs();

            DebugMode = Config.Bind("Custom", "Enable Debug Mode", "false");
            GhostsSeeInformation = Config.Bind("Custom", "Ghosts See Remaining Tasks", true);
            GhostsSeeRoles = Config.Bind("Custom", "Ghosts See Roles", true);
            GhostsSeeModifier = Config.Bind("Custom", "Ghosts See Modifier", true);
            GhostsSeeVotes = Config.Bind("Custom", "Ghosts See Votes", true);
            ShowRoleSummary = Config.Bind("Custom", "Show Role Summary", true);
            ShowLighterDarker = Config.Bind("Custom", "Show Lighter / Darker", true);
            ToggleCursor = Config.Bind("Custom", "Better Cursor", true);
            EnableSoundEffects = Config.Bind("Custom", "Enable Sound Effects", true);
            EnableHorseMode = Config.Bind("Custom", "Enable Horse Mode", false);
            ShowPopUpVersion = Config.Bind("Custom", "Show PopUp", "0");
            ShowChatNotifications = Config.Bind("Custom", "Show Chat Notifications", true);
            ShowVentsOnMap = Config.Bind("Custom", "Show Vent Positions On Minimap", false);
            ShowExtraInfo = Config.Bind("Custom", "Show Info On Mouse Hover", true);

            Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
            Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
            defaultRegions = ServerManager.DefaultRegions;

            ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
            UpdateRegions();

            DebugMode = Config.Bind("Custom", "Enable Debug Mode", "false");
            Harmony.PatchAll();
            
            CustomOptionHolder.Load();
            CustomColors.Load();
            CustomHatManager.LoadHats();
            AssetLoader.LoadAssets();
            Achievement.LoadAchievements();
            EventDetail.Load();
            TranslatableTag.Load();
            if (ToggleCursor.Value) Helpers.enableCursor(true);
            if (BepInExUpdater.UpdateRequired)
            {
                AddComponent<BepInExUpdater>();
                return;
            }

            EventUtility.Load();
            SubmergedCompatibility.Initialize();
            AddComponent<ModUpdateBehaviour>();
            Modules.MainMenuPatch.addSceneChangeCallbacks();
            SceneManager.sceneLoaded += (UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)((scene, loadMode) =>
            {
                new GameObject("TORManager").AddComponent<TORGUIManager>();
            });

            ClassInjector.RegisterTypeInIl2Cpp(typeof(FoxTask));
        }
        public static Sprite GetModStamp() {
            if (ModStamp) return ModStamp;
            return ModStamp = Helpers.loadSpriteFromResources("TheDoraemon.Resources.ModStamp.png", 150f);
        }
    }

    // Deactivate bans, since I always leave my local testing game and ban myself
    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static class AmBannedPatch
    {
        public static void Postfix(out bool __result)
        {
            __result = false;
        }
    }
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
    public static class ChatControllerAwakePatch {
        private static void Prefix() {
            if (!EOSManager.Instance.isKWSMinor) {
                DataManager.Settings.Multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
            }
        }
    }
    
    // Debugging tools
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class DebugManager
    {
        private static readonly string passwordHash = "d1f51dfdfd8d38027fd2ca9dfeb299399b5bdee58e6c0b3b5e9a45cd4e502848";
        private static readonly System.Random random = new((int)DateTime.Now.Ticks);
        private static List<PlayerControl> bots = new();

        public static void Postfix(KeyboardJoystick __instance)
        {
            // Check if debug mode is active.
            StringBuilder builder = new();
            SHA256 sha = SHA256Managed.Create();
            Byte[] hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(TheDoraemonPlugin.DebugMode.Value));
            foreach (var b in hashed) {
                builder.Append(b.ToString("x2"));
            }
            string enteredHash = builder.ToString();
            if (enteredHash != passwordHash) return;


            // Spawn dummys
            /*if (Input.GetKeyDown(KeyCode.F)) {
                var playerControl = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);
                var i = playerControl.PlayerId = (byte) GameData.Instance.GetAvailableId();

                bots.Add(playerControl);
                GameData.Instance.AddPlayer(playerControl);
                AmongUsClient.Instance.Spawn(playerControl, -2, InnerNet.SpawnFlags.None);
                
                playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
                playerControl.GetComponent<DummyBehaviour>().enabled = true;
                playerControl.NetTransform.enabled = false;
                playerControl.SetName(RandomString(10));
                playerControl.SetColor((byte) random.Next(Palette.PlayerColors.Length));
                GameData.Instance.RpcSetTasks(playerControl.PlayerId, new byte[0]);
            }*/

            // Terminate round
            if(Input.GetKeyDown(KeyCode.L)) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ForceEnd, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.forceEnd();
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
