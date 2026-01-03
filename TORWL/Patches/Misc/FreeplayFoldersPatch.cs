using HarmonyLib;
using UnityEngine;
using MiraAPI.Roles;
using TORWL.Roles.Coven;
using TORWL.Features;

namespace TORWL.Patches.Misc
{
    [HarmonyPatch(typeof(TaskAdderGame))]
    internal static class CovenFreeplayStylingPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TaskAdderGame.ShowFolder))]
        private static void StyleCovenFolder(TaskFolder taskFolder)
        {
            if (taskFolder == null)
                return;

            if (taskFolder.FolderName != "Coven")
                return;

            taskFolder.currentFolderColor = LaunchpadPalette.Coven;
            taskFolder.folderSpriteRenderer.color = LaunchpadPalette.Coven;
            taskFolder.buttonRolloverHandler.OutColor = LaunchpadPalette.Coven;
            taskFolder.buttonRolloverHandler.UnselectedColor = LaunchpadPalette.Coven;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TaskAdderGame.ShowFolder))]
        private static void StyleCovenFiles(TaskAdderGame __instance)
        {
            foreach (var t in __instance.ActiveItems)
            {
                var button = t.GetComponent<TaskAddButton>();
                if (button == null)
                    continue;

                if (button.role is not ICovenRole)
                    continue;

                button.FileImage.sprite = LaunchpadAssets.CovenFile.LoadAsset();
                button.FileImage.color = LaunchpadPalette.Coven;
                button.RolloverHandler.OutColor = LaunchpadPalette.Coven;
            }
        }
    }
}
