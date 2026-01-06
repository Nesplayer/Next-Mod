using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TORWL.Modifiers;
using TORWL.Options.Roles.Neutral;
using TORWL.Roles.Neutral;
using UnityEngine;
using TORWL.Features;
using Il2CppSystem;
using MiraAPI.Utilities;

namespace TORWL.Buttons.Neutral;
public sealed class VestButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Safeguard";
    public override Color TextOutlineColor => LaunchpadPalette.SurvivorColor;
    public override float Cooldown => OptionGroupSingleton<SurvivorOptions>.Instance.VestCooldown;
    public override float EffectDuration => OptionGroupSingleton<SurvivorOptions>.Instance.VestDuration;
    public override int MaxUses => (int)OptionGroupSingleton<SurvivorOptions>.Instance.MaxVests;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.Vest;
    public override bool TimerAffectedByPlayer => true;
    public override bool Enabled(RoleBehaviour? role) => role is SurvivorRole;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Player.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.SurvivorColor;
    }
    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer;
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.SurvivorColor));
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcAddModifier<SurvivorVestModifier>();
    }
}