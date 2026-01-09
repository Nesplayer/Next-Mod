using Il2CppInterop.Runtime.Attributes;
using TORWL.Features;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TORWL.Roles.Impostor;

public class BurrowerRole(IntPtr ptr) : ImpostorRole(ptr), IImpostorRole
{
    public string RoleName => "Burrower";
    public string RoleDescription => "Create vents around the map.";
    public string RoleLongDescription => "Move around the map easier\nBy digging new vents.";
    public Color RoleColor => LaunchpadPalette.BurrowerColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public TORWLFactions Faction => TORWLFactions.Stealth;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = LaunchpadAssets.Burrower,
    };

    [HideFromIl2Cpp]
    public List<Vent> DugVents { get; } = [];
    
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.SpawnImpostorTaskHeader();
    }
}
