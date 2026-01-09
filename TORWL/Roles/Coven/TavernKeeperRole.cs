using TORWL.Options.Roles.Coven;
using TORWL.Features;
using Il2CppSystem.Text;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;

namespace TORWL.Roles.Coven;

public class TavernKeeperRole(System.IntPtr ptr) : RoleBehaviour(ptr), ICovenRole
{

    public string RoleName => "Tavern Keeper";
    public string RoleDescription => "Use magic against the crew.";
    public string RoleLongDescription => "Use your role block ability to\nstop players from using their ability";
    public Color RoleColor => LaunchpadPalette.TavernKeeperColor;
    public TORWLFactions Faction => TORWLFactions.Dominion;
    public override bool IsDead => false;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanGetKilled = true,
        Icon = LaunchpadAssets.TavernKeeper,
        FreeplayFolder = "Coven",
    };

    public override void AppendTaskHint(StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }
    
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.SpawnCovenTaskHeader();
    }
}