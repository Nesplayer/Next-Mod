using Il2CppSystem;
using TORWL.Features;
using TORWL.Options.Roles.Crewmate;
using TORWL.Roles.Crewmate;
using TORWL.Translations;
using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using Rewired;
using UnityEngine;

namespace TORWL.Buttons.Crewmate;

public class ShootButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => ButtonName.GetTranslatedText();
    public override float Cooldown => OptionGroupSingleton<SheriffOptions>.Instance.ShotCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)OptionGroupSingleton<SheriffOptions>.Instance.ShotsPerGame;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.ShootButton;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role) => role is SheriffRole;

    public TranslationPool ButtonName = new TranslationPool(
        english: "Shoot",
        spanish: "Disparar",
        french: "Tirer"
    );

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = LaunchpadAssets.Player.LoadAsset();
        Button!.usesRemainingSprite.color = LaunchpadPalette.SheriffColor;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, 1.1f);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.SheriffColor));
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (Target.Data.Role.TeamType == RoleTeamTypes.Impostor || OptionGroupSingleton<SheriffOptions>.Instance.ShouldCrewmateDie && Target.Data.Role.TeamType == RoleTeamTypes.Crewmate)
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(Target);
        }

        if (Target.Data.Role.TeamType == RoleTeamTypes.Crewmate)
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(PlayerControl.LocalPlayer);
        }

        ResetTarget();
    }
}