using HarmonyLib;
using UnityEngine;
using MiraAPI.Utilities.Assets;

namespace TORWL.Features.Wiki
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class WikiKeybindPatch
    {
        public static void Postfix(HudManager __instance)
        {
            // Only allow opening if the player is actually in a game/lobby
            if (AmongUsClient.Instance == null) return;

            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (WikiPanel.Instance == null)
                {
                    OpenWiki(__instance);
                }
                else
                {
                    WikiPanel.Instance.Close();
                }
            }
        }

        private static void OpenWiki(HudManager hud)
        {
            // Load prefab from your AssetBundle
            GameObject prefab = LaunchpadAssets.WikiPrefab?.LoadAsset();
            if (prefab == null)
            {
                Debug.LogError("Wiki Prefab not found in LaunchpadAssets!");
                return;
            }

            // Instantiate and set parent to the HUD so it scales with the screen
            GameObject wikiObj = Object.Instantiate(prefab, hud.transform);
            wikiObj.transform.localPosition = Vector3.zero;
            wikiObj.transform.localScale = Vector3.one;

            // Add the script so the buttons and search start working
            wikiObj.AddComponent<WikiPanel>();
        }
    }
}