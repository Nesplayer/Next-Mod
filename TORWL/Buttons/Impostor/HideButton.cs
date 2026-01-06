using TORWL.Components;
using TORWL.Features;
using TORWL.Modifiers;
using TORWL.Networking;
using TORWL.Networking.Roles;
using TORWL.Roles.Impostor;
using TORWL.Utilities;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraAPI.Keybinds;
using Rewired;
using UnityEngine;
using Helpers = MiraAPI.Utilities.Helpers;

namespace TORWL.Buttons.Impostor;

public class HideButton : BaseLaunchpadButton<DeadBody>
{
    public override string Name => "HIDE";
    public override float Cooldown => 5;
    public override float EffectDuration => 0;
    public override int MaxUses => 3;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.HideButton;
    public override float Distance => PlayerControl.LocalPlayer.MaxReportDistance / 4f;
    public override bool TimerAffectedByPlayer => true;

    public override BaseKeybind Keybind => MiraGlobalKeybinds.SecondaryAbility;

    private static Vent VentTarget => HudManager.Instance.ImpostorVentButton.currentTarget;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Body.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.JanitorColor;
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

    public override bool CanUse()
    {
        return base.CanUse() &&
               PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>() &&
               VentTarget && !VentTarget.gameObject.GetComponent<VentBodyComponent>()
               && !VentTarget.IsSealed();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        PlayerControl.LocalPlayer.RpcStopDragging();
        PlayerControl.LocalPlayer.RpcHideBodyInVent(Target.ParentId, VentTarget.Id);

        ResetTarget();
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is JanitorRole;
    }
}