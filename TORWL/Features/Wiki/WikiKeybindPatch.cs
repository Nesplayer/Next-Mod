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
            GameObject? prefab = LaunchpadAssets.WikiPrefab?.LoadAsset();
            if (prefab == null)
            {
                Debug.LogError("Wiki Prefab not found in LaunchpadAssets!");
                return;
            }

            GameObject wikiObj = Object.Instantiate(prefab, hud.transform);
            wikiObj.transform.localPosition = Vector3.zero;
            wikiObj.transform.localScale = Vector3.one;

            wikiObj.AddComponent<WikiPanel>();
        }
    }
}