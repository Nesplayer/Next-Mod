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

        try
        {
            string pluginPath = Paths.PluginPath;
            UnityEngine.Debug.Log($"[TORWL] Paths.PluginPath = {pluginPath}");

            string folder = Path.Combine(pluginPath, "TORWL");
            UnityEngine.Debug.Log($"[TORWL] Target folder path = {folder}");

            Directory.CreateDirectory(folder);

            bool folderExists = Directory.Exists(folder);
            UnityEngine.Debug.Log($"[TORWL] Folder exists after CreateDirectory(): {folderExists}");

            string filePath = Path.Combine(folder, "welcome.txt");
            UnityEngine.Debug.Log($"[TORWL] Target file path = {filePath}");

            string defaultText =
            @"Welcome to the lobby!\n<b>Have fun!</b>\n<color=#00FF00>Enjoy the game</color>";

            if (!File.Exists(filePath))
            {
                UnityEngine.Debug.Log("[TORWL] welcome.txt does not exist, creating it now...");
                File.WriteAllText(filePath, defaultText);
            }
            else
            {
                UnityEngine.Debug.Log("[TORWL] welcome.txt already exists.");
            }

            bool fileExists = File.Exists(filePath);
            UnityEngine.Debug.Log($"[TORWL] File exists after WriteAllText(): {fileExists}");

            string welcomeText = File.ReadAllText(filePath);
            UnityEngine.Debug.Log($"[TORWL] File contents loaded successfully:\n{welcomeText}");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[TORWL] FILE IO ERROR:\n{ex}");
        }

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