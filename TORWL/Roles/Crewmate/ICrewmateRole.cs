using TORWL.Features;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using TORWL.Utilities;

namespace TORWL.Roles.Crewmate
{
    public interface ICrewmateRole : ICustomRole, IOptionable
    {
        ModdedRoleTeams ICustomRole.Team => ModdedRoleTeams.Crewmate;

        RoleOptionsGroup ICustomRole.RoleOptionsGroup => 
            new RoleOptionsGroup("♦ Crewmate Roles ♦", new Color32(120, 204, 236, byte.MaxValue), -2);

        TeamIntroConfiguration? ICustomRole.IntroConfiguration => 
            new TeamIntroConfiguration(Color.cyan, "CREWMATE", 
                "You are a Crewmate. Do tasks and vote off the " + 
                Extensions.ToTextColor(Palette.ImpostorRed) + "Impostor</color>.");
        
        public TORWLFactions Faction { get; }
        public static StringBuilder GetRoleTabText(ICustomRole role)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{role.RoleColor.ToTextColor()}You are <b>{role.RoleName}</b></color>");
            sb.AppendLine($"<size=65%>Faction: {Utils.GetCrewmateFactionDisplay((ICrewmateRole)role)}</size>");
            sb.AppendLine($"<size=70%>{role.RoleLongDescription}</size>");
            return sb;
        }

        [HideFromIl2Cpp]
        StringBuilder ICustomRole.SetTabText()
        {
            return GetRoleTabText(this);
        }
    }
    public static class CrewmateRoleExtensions
    {
        public static void SpawnCrewmateTaskHeader(this PlayerControl playerControl)
        {
            if (playerControl != PlayerControl.LocalPlayer) return;

            var orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl);
            orCreateTask.Text = string.Concat([
                LaunchpadPalette.Crewmate.ToTextColor(),
                "Do tasks and vote off the " +
                Extensions.ToTextColor(Palette.ImpostorRed) + "Impostor(s)</color>" +
                " to win!</color>"
            ]);
        }
    }
}