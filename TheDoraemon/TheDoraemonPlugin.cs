using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities;

namespace TheDoraemon;

[BepInPlugin("遨游屑", "TheDoraemon", "v1.0.0.0")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class TheDoraemonPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public const string Id = "TheDoraemon";
    public const string VersionString = "1.0.0.0";
    public static TheDoraemonPlugin Instance;
    public ConfigEntry<string> ConfigName { get; private set; }
    public static int optionsPage = 2;

    public override void Load()
    {
        //ConfigName = Config.Bind("Fake", "Name", ":>");

        Harmony.PatchAll();
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class ExamplePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            __instance.cosmetics.nameText.text = PluginSingleton<TheDoraemonPlugin>.Instance.ConfigName.Value;
        }
    }
}
