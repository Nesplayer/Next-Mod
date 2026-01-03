using HarmonyLib;
using TORWL.Features;
using TORWL.Features.Wiki;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Rewired.Platforms.Custom;
using MiraAPI.Utilities;

namespace TORWL.Patches.Misc
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuPatch
    {

        public static void Postfix(MainMenuManager __instance)
        {
            SetButtonColor(__instance.playButton, true);
            SetButtonColor(__instance.inventoryButton, true);
            SetButtonColor(__instance.shopButton, true);
            SetButtonColor(__instance.newsButton, false);
            SetButtonColor(__instance.myAccountButton, false);
            SetButtonColor(__instance.accountCTAButton, true);
            SetButtonColor(__instance.accountStatsButton, true);
            SetButtonColor(__instance.settingsButton, false);
            SetButtonColor(__instance.creditsButton, false);
            SetButtonColor(__instance.quitButton, false);
        }

        private static void SetButtonColor(PassiveButton Button, bool shine)
        {
            Button.activeSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.inactiveSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue.DarkenColor();
            Button.activeTextColor = new Color(0f, 0f, 0.4528f, 1f);
            Button.inactiveTextColor = new Color(0f, 0.3544f, 1f, 1f);
            if (shine)
            {
                Button.activeSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
                Button.inactiveSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
            }

        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class PlayerOptionsCubePatch
    {
        public static void Postfix(GameSettingMenu __instance)
        {
            var cube = __instance.transform.Find("What Is This?/Cube");

            if (cube == null)
            {
                Debug.LogError("[Launchpad] Could not find Cube inside What Is This?");
                return;
            }

            var sr = cube.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogError("[Launchpad] Cube has no SpriteRenderer!");
                return;
            }

            sr.color = Palette.CrewmateRoleBlue.DarkenColor();
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    public static class LobbyPopoutTabsPatch
    {
        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            // Find buttons
            var overviewTab = __instance.transform.Find("OverviewTab")?.GetComponent<PassiveButton>();
            var rolesTab = __instance.transform.Find("RolesTabs")?.GetComponent<PassiveButton>();
            var modifiersTab = __instance.transform.Find("ModifiersTabButton")?.GetComponent<PassiveButton>();

            if (overviewTab != null)
                PatchTabButton(overviewTab, LaunchpadAssets.ViewIcon.LoadAsset());

            if (rolesTab != null)
                PatchTabButton(rolesTab, LaunchpadAssets.RolesIcon.LoadAsset());

            if (modifiersTab != null)
                PatchTabButton(modifiersTab, LaunchpadAssets.ModifiersIcon.LoadAsset());
        }


        private static void PatchTabButton(PassiveButton Button, Sprite icon)
        {
            // Colors
            Button.activeSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.inactiveSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue.DarkenColor();
            Button.selectedSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;

            Button.activeTextColor = new Color(0f, 0f, 0.4528f, 1f);
            Button.inactiveTextColor = new Color(0f, 0.3544f, 1f, 1f);
            Button.selectedTextColor = new Color(0f, 0f, 0.4528f, 1f);

            // Icon slots (child 0 == icon)
            var activeIcon = Button.activeSprites.transform.GetChild(0);
            var inactiveIcon = Button.inactiveSprites.transform.GetChild(0);
            var selectedIcon = Button.selectedSprites != null ? Button.selectedSprites.transform.GetChild(0) : null;

            if (icon != null)
            {
                activeIcon.GetComponent<SpriteRenderer>().sprite = icon;
                inactiveIcon.GetComponent<SpriteRenderer>().sprite = icon;

                if (selectedIcon != null)
                    selectedIcon.GetComponent<SpriteRenderer>().sprite = icon;
            }

            // Scale identical to your other buttons
            activeIcon.localScale = new Vector3(0.45f, 0.45f, 1f);
            inactiveIcon.localScale = new Vector3(0.45f, 0.45f, 1f);

            if (selectedIcon != null)
                selectedIcon.localScale = new Vector3(0.45f, 0.45f, 1f);

            // Enable icons
            activeIcon.gameObject.SetActive(true);
            inactiveIcon.gameObject.SetActive(true);
            if (selectedIcon != null) selectedIcon.gameObject.SetActive(true);

            // Remove the shine layer (child 1)
            Button.activeSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
            Button.inactiveSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;

            if (Button.selectedSprites != null)
                Button.selectedSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public static class GameStartPatch
    {

        public static void Postfix(GameStartManager __instance)
        {
            HostViewButtonColor(__instance.HostViewButton, true);
            EditButtonColor(__instance.EditButton, true);
            ClientViewButtonColor(__instance.ClientViewButton, true);
            if (__instance.LobbyInfoPane != null)
                PaneColor(__instance.LobbyInfoPane);
        }

        private static void HostViewButtonColor(PassiveButton Button, bool shine)
        {
            Button.activeSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.inactiveSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue.DarkenColor();
            Button.activeTextColor = new Color(0f, 0f, 0.4528f, 1f);
            Button.inactiveTextColor = new Color(0f, 0.3544f, 1f, 1f);
            Button.selectedSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.selectedTextColor = new Color(0.0667f, 0.4494f, 0.7358f, 1f);

            var activeSprite = Button.activeSprites.transform.GetChild(0);
            var inactiveSprite = Button.inactiveSprites.transform.GetChild(0);
            var selectedSprite = Button.selectedSprites.transform.GetChild(0);

            var icon = LaunchpadAssets.ViewIcon.LoadAsset();
            activeSprite.GetComponent<SpriteRenderer>().sprite = icon;
            inactiveSprite.GetComponent<SpriteRenderer>().sprite = icon;
            selectedSprite.GetComponent<SpriteRenderer>().sprite = icon;

            activeSprite.localScale = new Vector3(0.45f, 0.45f, 1f);
            inactiveSprite.localScale = new Vector3(0.45f, 0.45f, 1f);
            selectedSprite.localScale = new Vector3(0.45f, 0.45f, 1f);

            SetDistanceFromEdge(activeSprite, new Vector3(0.46f, 0f, -1f));
            SetDistanceFromEdge(inactiveSprite, new Vector3(0.46f, 0f, -1f));
            SetDistanceFromEdge(selectedSprite, new Vector3(0.46f, 0f, -1f));

            activeSprite.gameObject.SetActive(true);
            inactiveSprite.gameObject.SetActive(true);
            selectedSprite.gameObject.SetActive(true);

            if (shine)
            {
                Button.activeSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
                Button.inactiveSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        private static void ClientViewButtonColor(PassiveButton Button, bool shine)
        {
            Button.activeSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.inactiveSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue.DarkenColor();
            Button.activeTextColor = new Color(0f, 0f, 0.4528f, 1f);
            Button.inactiveTextColor = new Color(0f, 0.3544f, 1f, 1f);
            Button.selectedSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.selectedTextColor = new Color(0.0667f, 0.4494f, 0.7358f, 1f);

            var activeSprite = Button.activeSprites.transform.GetChild(0);
            var inactiveSprite = Button.inactiveSprites.transform.GetChild(0);

            var icon = LaunchpadAssets.ViewIcon.LoadAsset();
            activeSprite.GetComponent<SpriteRenderer>().sprite = icon;
            inactiveSprite.GetComponent<SpriteRenderer>().sprite = icon;

            activeSprite.localScale = new Vector3(0.45f, 0.45f, 1f);
            inactiveSprite.localScale = new Vector3(0.45f, 0.45f, 1f);

            activeSprite.gameObject.SetActive(true);
            inactiveSprite.gameObject.SetActive(true);

            if (shine)
            {
                Button.activeSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
                Button.inactiveSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        private static void EditButtonColor(PassiveButton Button, bool shine)
        {
            Button.activeSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue;
            Button.inactiveSprites.GetComponent<SpriteRenderer>().color = Palette.CrewmateRoleBlue.DarkenColor();
            Button.activeTextColor = new Color(0f, 0f, 0.4528f, 1f);
            Button.inactiveTextColor = new Color(0f, 0.3544f, 1f, 1f);

            var activeSprite = Button.activeSprites.transform.GetChild(0);
            var inactiveSprite = Button.inactiveSprites.transform.GetChild(0);

            var icon = LaunchpadAssets.EditIcon.LoadAsset();
            activeSprite.GetComponent<SpriteRenderer>().sprite = icon;
            inactiveSprite.GetComponent<SpriteRenderer>().sprite = icon;

            activeSprite.localScale = new Vector3(0.45f, 0.45f, 1f);
            inactiveSprite.localScale = new Vector3(0.45f, 0.45f, 1f);

            SetDistanceFromEdge(activeSprite, new Vector3(0.46f, 0f, -1f));
            SetDistanceFromEdge(inactiveSprite, new Vector3(0.46f, 0f, -1f));

            activeSprite.gameObject.SetActive(true);
            inactiveSprite.gameObject.SetActive(true);

            if (shine)
            {
                Button.activeSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
                Button.inactiveSprites.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.clear;
            }
        }

        private static void SetDistanceFromEdge(Transform icon, Vector3 pos)
        {
            var aspect = icon.GetComponent<AspectPosition>();
            if (aspect != null)
            {
                aspect.DistanceFromEdge = pos;
                aspect.AdjustPosition(); // force update
            }
        }

        private static void PaneColor(MonoBehaviour paneComponent)
        {
            if (paneComponent == null) return;

            var pane = paneComponent.gameObject;

            var background = pane.transform.Find("AspectSize/Background");
            if (background == null) return;

            var sr = background.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            sr.color = Palette.CrewmateRoleBlue;
        }
    }
}