using TORWL.Features;
using TORWL.Modifiers;
using TORWL.Options.Roles.Impostor;
using TORWL.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using UnityEngine;

namespace TORWL.Buttons.Impostor;

public class SilenceButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Silence";
    public override Color TextOutlineColor => LaunchpadPalette.SilencerColor;
    public override float Cooldown => OptionGroupSingleton<SilencerOptions>.Instance.SilenceCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<SilencerOptions>.Instance.SilenceUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.SilenceButton;
    public override bool TimerAffectedByPlayer => true;

    public override BaseKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is SilencerRole;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Player.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.SilencerColor;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance, false);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(LaunchpadPalette.SilencerColor));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return true;
    }
    
    protected override void OnClick()
    {
        if (Target != null)
        {
            Target.AddModifier<SilencedModifier>();
        }
    }
}