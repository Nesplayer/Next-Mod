using TORWL.Roles.Coven;
using TORWL.Features;
using TORWL.Modifiers;
using TORWL.Options.Roles.Coven;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using UnityEngine;
using MiraAPI.Utilities.Assets;
using MiraAPI.Utilities;

namespace TORWL.Buttons.Coven;
public class RoleBlockButton : BaseLaunchpadButton<PlayerControl>
{

    public override string Name => "Role Block";
    public override Color TextOutlineColor => LaunchpadPalette.TavernKeeperColor;
    public override float Cooldown => OptionGroupSingleton<TavernKeeperOptions>.Instance.RoleBlockCooldown;
    public override float EffectDuration => OptionGroupSingleton<TavernKeeperOptions>.Instance.RoleBlockDuration;
    public override int MaxUses => (int)OptionGroupSingleton<TavernKeeperOptions>.Instance.RoleBlockUses;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.RoleBlockButton;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is TavernKeeperRole;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Player.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.TavernKeeperColor;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance, false);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(LaunchpadPalette.TavernKeeperColor));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return true;
    }
    protected override void OnClick()
    {
        if (Target != null)
        {
            Target.AddModifier<RoleBlockedModifier>();
        }

        SoundManager.Instance.PlaySound(LaunchpadAudio.MagicWhoosh.LoadAsset(), false, volume: 5);
    }

    public override void OnEffectEnd()
    {
    }
}