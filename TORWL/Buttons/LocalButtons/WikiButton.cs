using MiraAPI.Keybinds;
using TORWL.Features;
using MiraAPI.Utilities.Assets;
using TORWL.Buttons.Keybinds;
using TORWL.Features.Wiki;
using UnityEngine;

namespace TORWL.Buttons;

public class WikiButton : BaseLaunchpadButton
{
    public override string Name => "Open Wiki";
    public override Color TextOutlineColor => new Color32(89, 223, 231, 255);
    public override float Cooldown => 0;
    public override float EffectDuration => 0;
    public override int MaxUses => 0;
    public override BaseKeybind Keybind => TORWLKeybinds.OpenWiki;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.WikiButton;
    public override bool TimerAffectedByPlayer => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null && TutorialManager.InstanceExists;
    }

    protected override void OnClick()
    {
        if (HudManager.Instance == null)
            return;

        WikiOpener.Toggle(HudManager.Instance);
    }
}