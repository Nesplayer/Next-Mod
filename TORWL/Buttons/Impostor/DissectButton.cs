using TORWL.Features;
using TORWL.Networking.Roles;
using TORWL.Options.Roles.Impostor;
using TORWL.Roles.Impostor;
using TORWL.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraAPI.Keybinds;
using Rewired;
using UnityEngine;
using Helpers = MiraAPI.Utilities.Helpers;

namespace TORWL.Buttons.Impostor;

public class DissectButton : BaseLaunchpadButton<DeadBody>
{
    public override string Name => "Dissect";
    public override float Cooldown => OptionGroupSingleton<SurgeonOptions>.Instance.DissectCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<SurgeonOptions>.Instance.DissectUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.DissectButton;
    public override bool TimerAffectedByPlayer => true;

    public override BaseKeybind Keybind => MiraGlobalKeybinds.SecondaryAbility;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is SurgeonRole;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Body.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.SurgeonColor;
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestObjectOfType<DeadBody>(Distance, Helpers.CreateFilter(Constants.NotShipMask), "DeadBody", IsTargetValid);
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target != null && target.GetCacheComponent() is
        {
            hidden: false
        };
    }

    public override void SetOutline(bool active)
    {
        if (Target == null)
        {
            return;
        }

        foreach (var renderer in Target.bodyRenderers)
        {
            renderer.UpdateOutline(active ? PlayerControl.LocalPlayer.Data.Role.NameColor : null);
        }
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        SoundManager.Instance.PlaySound(LaunchpadAssets.DissectSound.LoadAsset(), false, volume: 2);
        PlayerControl.LocalPlayer.RpcDissect(Target.ParentId);

        ResetTarget();
    }
}