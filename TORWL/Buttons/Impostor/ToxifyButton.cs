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

public class ToxifyButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Toxify";
    public override Color TextOutlineColor => LaunchpadPalette.ToxifierColor;
    public override float Cooldown => OptionGroupSingleton<ToxifierOptions>.Instance.ToxifyCooldown;
    public override float EffectDuration => OptionGroupSingleton<ToxifierOptions>.Instance.ToxifiedDelay;
    public override int MaxUses => (int)OptionGroupSingleton<ToxifierOptions>.Instance.ToxifyUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.Toxify;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role) => role is ToxifierRole;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Player.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.ToxifierColor;
    }

    private PlayerControl? _toxifiedPlayer;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance, false);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return target != null && !target.HasModifier<PoisonModifier>();
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.ToxifierColor));
    }

    public override bool CanUse()
    {
        return base.CanUse() && _toxifiedPlayer == null;
    }

    public override void OnEffectEnd()
    {
        if (_toxifiedPlayer is null
            || _toxifiedPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.IsDead
            || (_toxifiedPlayer.Data.Role.IsImpostor && !OptionGroupSingleton<FunOptions>.Instance.FriendlyFire) || !_toxifiedPlayer.HasModifier<ToxifiedModifier>()
            || MeetingHud.Instance != null)
        {
            _toxifiedPlayer = null;
            return;
        }

        _toxifiedPlayer.RpcRemoveModifier<ToxifiedModifier>();
        PlayerControl.LocalPlayer.RpcCustomMurder(_toxifiedPlayer, resetKillTimer: false, createDeadBody: true, teleportMurderer: false, showKillAnim: false);
        _toxifiedPlayer = null;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        _toxifiedPlayer = Target;
        _toxifiedPlayer.RpcAddModifier<ToxifiedModifier>();

        SoundManager.Instance.PlaySound(LaunchpadAudio.Potion.LoadAsset(), false, volume: 5);

        ResetTarget();
    }
}