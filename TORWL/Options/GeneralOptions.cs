using MiraAPI.GameModes;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using System;
using TORWL.Features;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;
using MiraAPI.Utilities;

namespace TORWL.Options;

public class GeneralOptions : AbstractOptionGroup
{
    public override string GroupName => "General";
    public override Color GroupColor => LaunchpadPalette.GeneralMenu;
    public override Func<bool> GroupVisible => CustomGameModeManager.IsDefault;

    public ModdedToggleOption Notepad { get; set; } = new("Notepad", true)
    {
        ChangedEvent = value =>
        {
            NotepadHud.Instance?.SetNotepadButtonVisible(value);
        }
    };

    [ModdedToggleOption("Ban Cheaters")] public bool BanCheaters { get; set; } = true;
    [ModdedToggleOption("Disable Meeting Teleport")] public bool DisableMeetingTeleport { get; set; } = false;
    [ModdedToggleOption("Auto-Start Lobby")] public bool AutoStart { get; set; } = false;
    public ModdedNumberOption AutoStartAfter { get; } = new("Auto-Start after", 100f, 10f, 400, 10f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<GeneralOptions>.Instance.AutoStart,
    };
}