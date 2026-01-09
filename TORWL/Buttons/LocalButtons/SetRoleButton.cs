/*
using Il2CppSystem;
using TORWL.Components;
using TORWL.Features;
using TORWL.Modifiers;
using TORWL.Options.Roles.Crewmate;
using TORWL.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;

namespace TORWL.Buttons.Crewmate;

public class SetRoleButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Set Role";
    public override float Cooldown => 0;
    public override int MaxUses => 0;
    public override Color TextOutlineColor => new Color32(89, 223, 231, 255);
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.GambleButton;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null && TutorialManager.InstanceExists;
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestPlayer(true, 1.1f);

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.CrewMenu));
    }

    public override void ClickHandler()
    {
        if (CanClick())
            OnClick();
    }

    protected override void OnClick()
    {
        if (Target == null) return;

        var roleMenu = GuessRoleMinigame.Create();
        roleMenu.Open(
            role => !role.IsDead,
            selectedRole =>
            {
                if (selectedRole?.Role == null || Target?.Data == null) return;

                Target.RpcSetRole(selectedRole.Role);

                SoundManager.Instance.PlaySound(LaunchpadAssets.MoneySound.LoadAsset(), false, volume: 5);
                Target.RpcAddModifier<RevealedModifier>();

                roleMenu.Close();
            }
        );

        ResetTarget();
    }
}
 */