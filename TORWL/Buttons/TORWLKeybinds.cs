using MiraAPI.Keybinds;
using Rewired;

namespace TORWL.Buttons.Keybinds;

[RegisterCustomKeybinds]
public static class TORWLKeybinds
{
    public static MiraKeybind OpenWiki { get; } = new("Open Wiki\n<size=40%><color=#32A852>Freeplay</color></size", KeyboardKeyCode.C, [ModifierKey.Shift]);
}