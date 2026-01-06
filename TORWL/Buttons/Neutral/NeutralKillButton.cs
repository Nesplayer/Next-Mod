using TORWL.Features;
using TORWL.Options.Roles.Neutral;
using TORWL.Roles.Neutral;
using TORWL.Options;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using UnityEngine;
using Il2CppSystem;
using MiraAPI.Networking;
using MiraAPI.Utilities;

namespace TORWL.Buttons.Neutral;

public class NeutralKillButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Kill";
    public override float Cooldown => OptionGroupSingleton<NeutralKillerOptions>.Instance.NeutralKillCooldown;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.KillButton;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is NeutralKillerRole;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(OptionGroupSingleton<FunOptions>.Instance.FriendlyFire, 1.6f);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.NeutralKillerColor));
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }
        
        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }
}