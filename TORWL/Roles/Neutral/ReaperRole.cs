using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using TORWL.Features;
using TORWL.GameOver;
using TORWL.Options.Roles.Neutral;
using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;

namespace TORWL.Roles.Neutral;

public class ReaperRole(System.IntPtr ptr) : RoleBehaviour(ptr), INeutralRole
{
    public string RoleName => "Reaper";
    public string RoleDescription => "Collect souls to win";
    public string RoleLongDescription => "Collect souls from dead bodies to win the game.";
    public Color RoleColor => LaunchpadPalette.ReaperColor;
    public TORWLFactions Faction => TORWLFactions.Benign;
    public override bool IsDead => false;

    public int collectedSouls;

    public CustomRoleConfiguration Configuration => new(this)
    {
        TasksCountForProgress = false,
        CanUseVent = false,
        GhostRole = (RoleTypes)RoleId.Get<OutcastGhostRole>(),
        Icon = LaunchpadAssets.Reaper,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var sb = CustomRoleUtils.CreateForRole(this);
        sb.Append($"\n<b>{collectedSouls}/{OptionGroupSingleton<ReaperOptions>.Instance.SoulCollections} souls collected.");
        return sb;
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            playerControl.SpawnNeutralTaskHeader();
        }

    public override bool DidWin(GameOverReason reason)
    {
        return reason == CustomGameOver.GameOverReason<ReaperGameOver>();
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>();
        return !(console != null) || console.AllowImpostor;
    }
}
