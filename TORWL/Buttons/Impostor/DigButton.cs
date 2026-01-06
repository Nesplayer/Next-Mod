using TORWL.Features;
using TORWL.Networking.Roles;
using TORWL.Options.Roles.Impostor;
using TORWL.Roles.Impostor;
using TORWL.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using MiraAPI.Keybinds;
using Rewired;
using System.Linq;
using UnityEngine;

namespace TORWL.Buttons.Impostor;

public class DigButton : BaseLaunchpadButton
{
    public override string Name => "Dig Vent";
    public override float Cooldown => OptionGroupSingleton<BurrowerOptions>.Instance.VentDigCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<BurrowerOptions>.Instance.VentDigUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.DigVentButton;
    public override bool TimerAffectedByPlayer => true;

    private float _ventDist = OptionGroupSingleton<BurrowerOptions>.Instance.VentDist;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is BurrowerRole;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Vent.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.BurrowerColor;
    }

    public override bool CanUse()
    {
        return base.CanUse() && !ShipStatus.Instance.AllVents.Where(vent => !vent.IsSealed()).Any(vent =>
            Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), vent.transform.position) < _ventDist);
    }


    protected override void OnClick()
    {
        SoundManager.Instance.PlaySound(LaunchpadAssets.DigSound.LoadAsset(), false, volume: 2);
        PlayerControl.LocalPlayer.RpcDigVent();
    }
}