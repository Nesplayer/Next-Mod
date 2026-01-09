using TORWL.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace TORWL.Roles.Impostor;

public class SurgeonRole(IntPtr ptr) : ImpostorRole(ptr), IImpostorRole
{
    public string RoleName => "Surgeon";
    public string RoleDescription => "Poison other players and dissect bodies";
    public string RoleLongDescription => "You can poison players resulting in their death\nand you can dissect bodies to make them unreportable.";
    public Color RoleColor => LaunchpadPalette.SurgeonColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public TORWLFactions Faction => TORWLFactions.Saboteur;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Surgeon,
        UseVanillaKillButton = false,
    };
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.SpawnImpostorTaskHeader();
    }
}