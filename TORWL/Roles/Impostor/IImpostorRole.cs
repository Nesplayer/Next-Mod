using TORWL.Features;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using TORWL.Utilities;

namespace TORWL.Roles.Impostor
{
    public interface IImpostorRole : ICustomRole, IOptionable
    {
        ModdedRoleTeams ICustomRole.Team => ModdedRoleTeams.Impostor;

        RoleOptionsGroup ICustomRole.RoleOptionsGroup =>
            new RoleOptionsGroup("♦ Impostor Roles ♦", new Color32(203, 83, 84, byte.MaxValue), -1);

        TeamIntroConfiguration? ICustomRole.IntroConfiguration =>
            new TeamIntroConfiguration(Color.red, "IMPOSTOR", 
                "You are an Impostor. Sabotage and kill the crew.");
        
        public TORWLFactions Faction { get; }
        public static StringBuilder GetRoleTabText(ICustomRole role)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{role.RoleColor.ToTextColor()}You are <b>{role.RoleName}</b></color>");
            sb.AppendLine($"<size=65%>Faction: {Utils.GetImpostorFactionDisplay((IImpostorRole)role)}</size>");
            sb.AppendLine($"<size=70%>{role.RoleLongDescription}</size>");
            return sb;
        }

        [HideFromIl2Cpp]
        StringBuilder ICustomRole.SetTabText()
        {
            return GetRoleTabText(this);
        }
    }
    public static class ImpostorRoleExtensions
    {
        public static void SpawnImpostorTaskHeader(this PlayerControl playerControl)
        {
            if (playerControl != PlayerControl.LocalPlayer) return;

            var orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl);
            orCreateTask.Text = string.Concat([
                LaunchpadPalette.Impostor.ToTextColor(),
                "Sabotage and kill the " +
                Extensions.ToTextColor(Palette.CrewmateBlue) + "crew</color> " +
                "to win!</color>"
            ]);
        }
    }
}