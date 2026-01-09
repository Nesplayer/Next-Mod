using HarmonyLib;
using TORWL.Features;
using TORWL.Features.Wiki;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Rewired.Platforms.Custom;
using MiraAPI.Utilities;
using UnityEngine.Events;
using Object = UnityEngine.Object;

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
            
            AddClientWikiButton(__instance.ClientViewButton);
            AddHostWikiButton(__instance.HostViewButton);
        }
        
        private static PassiveButton? _clientWikiButton;
        private static PassiveButton? _hostWikiButton;

        private static void AddClientWikiButton(PassiveButton templateButton)
        {
            if (templateButton == null || _clientWikiButton != null)
                return;

            // Clone the button
            GameObject wikiObj = Object.Instantiate(templateButton.gameObject, templateButton.transform.parent);
            wikiObj.name = "WikiButton";

            _clientWikiButton = wikiObj.GetComponent<PassiveButton>();
            if (_clientWikiButton == null) return;

            // Destroy old UnityEvent entirely (clean slate)
            _clientWikiButton.OnClick.RemoveAllListeners();
            _clientWikiButton.OnClick = new Button.ButtonClickedEvent();

            // Add listener using a normal method
            _clientWikiButton.OnClick.AddListener(new Action(() =>
            {
                if (HudManager.Instance == null) return;
                WikiOpener.Toggle(HudManager.Instance);
            }));
            
            Sprite wikiIcon = LaunchpadAssets.WikiButton.LoadAsset(); // or your WikiIcon
            if (wikiIcon != null)
                SetWikiButtonIcon(_clientWikiButton, wikiIcon);

            // Change button text
            var text = wikiObj.GetComponentInChildren<TextMeshPro>(true);
            if (text != null)
            {
                var translator = text.GetComponent<TextTranslatorTMP>();
                if (translator != null)
                    Object.Destroy(translator);

                text.text = "Wiki";
            }

            // Nudge button so it doesn't overlap
            wikiObj.transform.localPosition = new Vector3(-0.063f, -0.905f, 0f);
        }
        
        private static void AddHostWikiButton(PassiveButton templateButton)
        {
            if (templateButton == null || _hostWikiButton != null)
                return;

            // Clone the button
            GameObject wikiObj = Object.Instantiate(templateButton.gameObject, templateButton.transform.parent);
            wikiObj.name = "WikiButton";

            _hostWikiButton = wikiObj.GetComponent<PassiveButton>();
            if (_hostWikiButton == null) return;

            // Destroy old UnityEvent entirely (clean slate)
            _hostWikiButton.OnClick.RemoveAllListeners();
            _hostWikiButton.OnClick = new Button.ButtonClickedEvent();

            // Add listener using a normal method
            _hostWikiButton.OnClick.AddListener(new Action(() =>
            {
                if (HudManager.Instance == null) return;
                WikiOpener.Toggle(HudManager.Instance);
            }));
            
            Sprite wikiIcon = LaunchpadAssets.WikiButton.LoadAsset(); // or your WikiIcon
            if (wikiIcon != null)
                SetWikiButtonIcon(_hostWikiButton, wikiIcon);

            // Change button text
            var text = wikiObj.GetComponentInChildren<TextMeshPro>(true);
            if (text != null)
            {
                var translator = text.GetComponent<TextTranslatorTMP>();
                if (translator != null)
                    Object.Destroy(translator);

                text.text = "Wiki";
            }

            // Nudge button so it doesn't overlap
            wikiObj.transform.localPosition = new Vector3(-0.963f, -0.905f, 0f);
        }
        
        private static void SetWikiButtonIcon(PassiveButton button, Sprite icon)
        {
            if (button.activeSprites != null && button.activeSprites.transform.childCount > 0)
            {
                var activeSprite = button.activeSprites.transform.GetChild(0).GetComponent<SpriteRenderer>();
                if (activeSprite != null) activeSprite.sprite = icon;
            }

            if (button.inactiveSprites != null && button.inactiveSprites.transform.childCount > 0)
            {
                var inactiveSprite = button.inactiveSprites.transform.GetChild(0).GetComponent<SpriteRenderer>();
                if (inactiveSprite != null) inactiveSprite.sprite = icon;
            }

            if (button.selectedSprites != null && button.selectedSprites.transform.childCount > 0)
            {
                var selectedSprite = button.selectedSprites.transform.GetChild(0).GetComponent<SpriteRenderer>();
                if (selectedSprite != null) selectedSprite.sprite = icon;
            }
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

            background.transform.localScale = new Vector3(1.0209f, 1.1209f, 1f);
            background.transform.localPosition = new Vector3(-1.9918f, -3.9459f, 0f);
        }
    }
}