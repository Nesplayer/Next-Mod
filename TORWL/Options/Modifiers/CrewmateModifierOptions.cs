using MiraAPI.GameOptions;
using TORWL.Features;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using UnityEngine;

namespace TORWL.Options.Modifiers;

public class CrewmateModifierOptions : AbstractOptionGroup
{
    public override string GroupName => "Crewmate Modifiers";
    public override bool ShowInModifiersMenu => true;
    public override Color GroupColor => LaunchpadPalette.CrewMenu;

    [ModdedNumberOption("Mayor Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float MayorChance { get; set; } = 0f;

    [ModdedNumberOption("Torch Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float TorchChance { get; set; } = 0f;
    
    [ModdedNumberOption("Bait Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float BaitChance { get; set; } = 0f;
    
    [ModdedNumberOption("Burst Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float BurstChance { get; set; } = 0f;
}