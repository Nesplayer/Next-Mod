using TORWL.Features;
using MiraAPI.Roles;
using TORWL.Components;
using System;
using UnityEngine;

namespace TORWL.Roles.Impostor
{
    // Corrected constructor syntax
    public class ToxifierRole : ImpostorRole, IImpostorRole
    {
        public ToxifierRole(IntPtr ptr) : base(ptr) { }

        public string RoleName => "Toxifier";
        public string RoleDescription => "Kill the crew by giving them deadly toxins.";
        public string RoleLongDescription => "You eliminate others by applying toxins instead of direct attacks.\nWhen you poison a player, they won\'t die instantly.\nYour goal is to infect targets without being noticed.\nUse timing, positioning, and subtle interactions to avoid suspicion while\nyour toxins finish the job.";
        public Color RoleColor => LaunchpadPalette.ToxifierColor;
        public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
        public TORWLFactions Faction => TORWLFactions.Stealth;

        public CustomRoleConfiguration Configuration => new(this)
        {
            Icon = LaunchpadAssets.Toxifier,
            UseVanillaKillButton = false,
        };

        public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            playerControl.SpawnImpostorTaskHeader();
        }
    }
}