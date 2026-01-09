using TORWL.Features;
using TORWL.Translations;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace TORWL.Roles.Crewmate;

public class SheriffRole(IntPtr ptr) : CrewmateRole(ptr), ICrewmateRole
{
    public string RoleName => "Sheriff";
    public string RoleDescription => "Take your chance by shooting a player.";
    public string RoleLongDescription => $"You can shoot players,\nif you shoot an {Palette.ImpostorRed.ToTextColor()}Impostor</color> you will kill him\nbut if you shoot a {Palette.CrewmateBlue.ToTextColor()}Crewmate</color>, you will die with him.";
    public Color RoleColor => LaunchpadPalette.SheriffColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public TORWLFactions Faction => TORWLFactions.Protective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Sheriff,
    };

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.SpawnCrewmateTaskHeader();
    }
}
