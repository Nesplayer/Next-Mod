using TORWL.Features;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using UnityEngine;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using TORWL.Utilities;

namespace TORWL.Roles.Neutral
{
    public interface INeutralRole : ICustomRole, IOptionable
    {
        ModdedRoleTeams ICustomRole.Team => ModdedRoleTeams.Custom;

        RoleOptionsGroup ICustomRole.RoleOptionsGroup =>
            new RoleOptionsGroup("♦ Neutral Roles ♦", Color.gray, 0);

        TeamIntroConfiguration? ICustomRole.IntroConfiguration =>
            new TeamIntroConfiguration(Color.gray, "NEUTRAL", 
                "You are a Neutral. You do not have a team.");
        
        public TORWLFactions Faction { get; }
        public static StringBuilder GetRoleTabText(ICustomRole role)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{role.RoleColor.ToTextColor()}You are <b>{role.RoleName}</b></color>");
            sb.AppendLine($"<size=65%>Faction: {Utils.GetNeutralFactionDisplay((INeutralRole)role)}</size>");
            sb.AppendLine($"<size=70%>{role.RoleLongDescription}</size>");
            return sb;
        }

        [HideFromIl2Cpp]
        StringBuilder ICustomRole.SetTabText()
        {
            return GetRoleTabText(this);
        }
    }
    public static class NeutralRoleExtensions
    {
        public static void SpawnNeutralTaskHeader(this PlayerControl playerControl)
        {
            if (playerControl != PlayerControl.LocalPlayer) return;

            var orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl);
            orCreateTask.Text = string.Concat([
                LaunchpadPalette.Neutral.ToTextColor(),
                "You have no team. Work alone to win",
                "</color>"
            ]);
        }
    }
}