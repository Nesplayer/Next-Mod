using TORWL.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace TORWL.Roles.Crewmate;

public class CoronerRole(IntPtr ptr) : CrewmateRole(ptr), ICrewmateRole
{
    public string RoleName => "Coroner";
    public string RoleDescription => "Freeze bodies to prevent them from disappearing.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => LaunchpadPalette.CoronerColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public TORWLFactions Faction => TORWLFactions.CrewSupport;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Coroner,
    };
    public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            playerControl.SpawnCrewmateTaskHeader();
        }
}