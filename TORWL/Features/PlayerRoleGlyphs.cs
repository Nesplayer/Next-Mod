using System;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TORWL.Features
{
    public class RoleIcons
    {
        public static bool CanLocalPlayerSeeRole(RoleBehaviour role)
        {
            ICustomRole? customRole = role as ICustomRole;
            if (customRole != null)
            {
                return customRole.CanLocalPlayerSeeRole(PlayerControl.LocalPlayer);
            }
            return (PlayerControl.LocalPlayer.Data.Role.IsImpostor && role.Player.Data.Role.IsImpostor) || PlayerControl.LocalPlayer.Data.IsDead;
        }

        [RegisterEvent(0)]
        private static void OnSetRole(SetRoleEvent e)
        {
            RoleIcons.TryClearRoleIcon(e.Player);
            RoleBehaviour role = e.Player.Data.Role;
            if (RoleIcons.CanLocalPlayerSeeRole(role))
            {
                GameObject gameObject = new GameObject("RoleIcon");
                gameObject.transform.SetParent(e.Player.cosmetics.nameTextContainer.transform);
                gameObject.transform.localPosition = new Vector3(0f, 0.4f, 0f);

                Sprite? roleIcon = RoleIcons.GetRoleIcon(role);
                if (roleIcon != null)
                {
                    gameObject.transform.localScale = RoleIcons.GetRoleIconScale(roleIcon, role is ICustomRole);
                    gameObject.AddComponent<SpriteRenderer>().sprite = roleIcon;
                }
            }
        }

        private static Vector3 GetRoleIconScale(Sprite icon, bool isCustomRole)
        {
            float num = isCustomRole ? 0.32f : 0.16f;
            return new Vector3((float)icon.texture.height / icon.pixelsPerUnit / num, (float)icon.texture.height / icon.pixelsPerUnit / num, 1f);
        }

        public static void TryClearRoleIcon(PlayerControl player)
        {
            Transform? transform = player.cosmetics.nameTextContainer.transform.FindChild("RoleIcon");
            GameObject? gameObject = (transform != null) ? transform.gameObject : null;

            if (gameObject != null)
            {
                gameObject.Destroy();
            }
        }

        public static Sprite? GetRoleIcon(RoleBehaviour role)
        {
            ICustomRole? customRole = role as ICustomRole;
            if (customRole == null)
            {
                return role.RoleIconSolid;
            }

            LoadableAsset<Sprite>? icon = customRole.Configuration.Icon;
            if (icon == null)
            {
                return null;
            }
            return icon.LoadAsset();
        }
    }
}