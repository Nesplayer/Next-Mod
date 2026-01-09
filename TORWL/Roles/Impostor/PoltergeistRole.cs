using System;
using TORWL.Features;
using TORWL.Roles.Afterlife;
using MiraAPI.Roles;
using UnityEngine;

namespace TORWL.Roles.Impostor;

public class PoltergeistRole(IntPtr ptr) : RoleBehaviour(ptr), IImpostorRole, IAfterlifeRole
{
    public string RoleName => "Poltergeist";
    public string RoleDescription => "Use your abilities to stop the crew from winning.\nYou are a <b>GHOST ROLE</b>.";
    public string RoleLongDescription => "When you get voted out, you have a chance to become the Poltergeist. Use your abilities to sabotage the crew so they are unable to win.\nThis is a <b>GHOST ROLE</b>";
    public Color RoleColor => LaunchpadPalette.PoltergeistColor;
    public TORWLFactions Faction => TORWLFactions.Saboteur;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Poltergeist,
        HideSettings = false,
        RoleHintType = RoleHintType.TaskHint,
        TasksCountForProgress = false,
        CanUseVent = false,
        ShowInFreeplay = true,
    };

    public override bool IsDead => true;
    public override bool IsAffectedByComms => false;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.ClearTasks();
        PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = $"{LaunchpadPalette.PoltergeistColor.ToTextColor()}You are dead, you cannot do tasks.\nUse your powers to stop the crew.";
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    public bool CanLocalPlayerSeeRole(PlayerControl player)
    {
        return false;
    }
}