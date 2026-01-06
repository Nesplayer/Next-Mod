using Il2CppSystem;
using TORWL.Features;
using TORWL.Modifiers;
using TORWL.Options;
using TORWL.Options.Roles.Impostor;
using TORWL.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TORWL.Buttons.Impostor;

public class CurseButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Curse";
    public override Color TextOutlineColor => LaunchpadPalette.PoltergeistColor;
    public override float Cooldown => OptionGroupSingleton<PoltergeistOptions>.Instance.CurseCooldown;
    public override float EffectDuration => OptionGroupSingleton<PoltergeistOptions>.Instance.CurseDuration;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.Curse;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role) => role is PoltergeistRole;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Player.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.PoltergeistColor;
    }

    private PlayerControl? _cursedPlayer;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance, false);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return target != null && !target.HasModifier<CursedModifier>();
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.PoltergeistColor));
    }

    public override bool CanUse()
    {
        return base.CanUse() && _cursedPlayer == null;
    }

    public override void OnEffectEnd()
    {
        if (_cursedPlayer is null
            || _cursedPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.IsDead
            || (_cursedPlayer.Data.Role.IsImpostor && !OptionGroupSingleton<FunOptions>.Instance.FriendlyFire) || !_cursedPlayer.HasModifier<CursedModifier>()
            || MeetingHud.Instance != null)
        {
            _cursedPlayer = null;
            return;
        }

        _cursedPlayer.RpcRemoveModifier<CursedModifier>();
        _cursedPlayer = null;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        _cursedPlayer = Target;
        _cursedPlayer.RpcAddModifier<CursedModifier>();

        SoundManager.Instance.PlaySound(LaunchpadAudio.Curse.LoadAsset(), false, volume: 5);

        ResetTarget();
    }
}