using TORWL.Features;
using TORWL.Options.Modifiers;
using TORWL.Options.Modifiers.Crewmate;
using TORWL.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using MiraAPI.Modifiers.Types;
using UnityEngine;

namespace TORWL.Modifiers.Game.Impostor;

public class ImpostorModifier : GameModifier
{
    public override string ModifierName => $"<color=#{LaunchpadPalette.Impostor.ToHtmlStringRGBA()}>Impostor Modifier</color>";
    public override LoadableAsset<Sprite>? ModifierIcon => LaunchpadAssets.ImpIcon;
    public override Color FreeplayFileColor => LaunchpadPalette.Impostor;

    public override string GetDescription() =>
        "A test modifier for the wiki";

    public override int GetAssignmentChance() => 100;

    public override int GetAmountPerGame() => 1;
    
    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor();
    }
}