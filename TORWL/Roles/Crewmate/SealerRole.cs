using Il2CppInterop.Runtime.Attributes;
using TORWL.Components;
using TORWL.Features;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TORWL.Roles.Crewmate;

public class SealerRole(IntPtr ptr) : CrewmateRole(ptr), ICrewmateRole
{
    public string RoleName => "Sealer";
    public string RoleDescription => "Seal vents around the map.";
    public string RoleLongDescription => "Seal vents around the map.\nThis will prevent anyone from entering the vent.";
    public Color RoleColor => LaunchpadPalette.SealerColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public TORWLFactions Faction => TORWLFactions.Support;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Sealer,
    };

    public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            playerControl.SpawnCrewmateTaskHeader();
        }

    [HideFromIl2Cpp]
    public List<SealedVentComponent> SealedVents { get; } = [];
}
