using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.IO;
using TORWL.Features;
using TORWL.Patches;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using System.Reflection;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TORWL.Patches.Misc;

namespace TORWL;

[BepInAutoPlugin("mod.angel.launchpad", "TOR-W: L")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[BepInDependency(CrowdedModPatch.CrowdedId, BepInDependency.DependencyFlags.SoftDependency)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class TORWLPlugin : BasePlugin, IMiraPlugin
{
    private Harmony Harmony { get; } = new(Id);

    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public string OptionsTitleText => "TOR-W:\nLaunchpad";
    public static string ModVersion
    {
        get
        {
            var full = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "Unknown";

            // Keep pre-release tag (e.g. -Dev-1), but remove metadata (+...)
            var clean = full.Split('+')[0];
            return clean;
        }
    }

    public static LaunchpadSettings? SettingsInstance;

    public static bool IsBetaBuild
    {
        get
        {
            var version = ModVersion.ToLowerInvariant();
            return version.Contains("-d") ||
                   version.Contains("-b") ||
                   version.Contains("-a") ||
                   version.Contains("-t");
        }
    }

    public override void Load()
    {
        SettingsInstance = new LaunchpadSettings(Config);

        Harmony.PatchAll();

        if (IsBetaBuild)
        {
            AddComponent<DebugWindow>();
            Log.LogInfo("DebugWindow ENABLED (beta build).");         // BepInEx log
            UnityEngine.Debug.Log("DebugWindow ENABLED (beta build)"); // In-game console
        }
        else
        {
            Log.LogInfo("DebugWindow DISABLED (release build).");
            UnityEngine.Debug.Log("DebugWindow DISABLED (release build)");
        }

        ReactorCredits.Register<TORWLPlugin>(ReactorCredits.AlwaysShow);

        IL2CPPChainloader.Instance.Finished += ModNewsFetcher.CheckForNews;

        Config.Save();
    }
}