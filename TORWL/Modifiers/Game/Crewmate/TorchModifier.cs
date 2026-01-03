using TORWL.Options.Modifiers;
using TORWL.Features;
using TORWL.Options.Modifiers.Crewmate;
using TORWL.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using Reactor.Utilities.Extensions;
using MiraAPI.Modifiers.Types;

namespace TORWL.Modifiers.Game.Crewmate;

public sealed class TorchModifier : GameModifier
{
    public override string ModifierName => $"<color=#{LaunchpadPalette.Torch.ToHtmlStringRGBA()}>Torch</color>";
    public override Color FreeplayFileColor => new Color32(255, 127, 50, 255);
    public override LoadableAsset<Sprite>? ModifierIcon => LaunchpadAssets.TorchIcon;
    public override string GetDescription() => 
        OptionGroupSingleton<TorchOptions>.Instance.UseFlashlight
        ? "You will have a flashlight\nif lights are sabotaged."
        : "You have max vision\nif lights are sabotaged.";

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.TorchChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<TorchOptions>.Instance.TorchAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }
}