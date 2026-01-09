using TORWL.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace TORWL.Roles.Crewmate;

public class GamblerRole(IntPtr ptr) : CrewmateRole(ptr), ICrewmateRole
{
    public string RoleName => "Gambler";
    public string RoleDescription => "Guess a player's role to reveal it!";
    public string RoleLongDescription => "Guess a player's role to reveal it!\nHowever, if you get it incorrect, you will die.";
    public Color RoleColor => LaunchpadPalette.GamblerColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public TORWLFactions Faction => TORWLFactions.Support;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Gambler,
    };
    public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            playerControl.SpawnCrewmateTaskHeader();
        }
}
