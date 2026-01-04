using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine.Events;
using UnityEngine.UI;
using Reactor.Utilities.Attributes;
using TORWL.Roles.Coven;
using TORWL.Roles.Neutral;
using TORWL.Roles.Crewmate;
using TORWL.Roles.Impostor;
using MiraAPI.Roles;
using MiraAPI.Modifiers.Types;
using TORWL.Modifiers;
using MiraAPI.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace TORWL.Features.Wiki
{
    [RegisterInIl2Cpp]
    public class WikiPanel : MonoBehaviour
    {
        public WikiPanel(IntPtr ptr) : base(ptr) { }

        public static WikiPanel? Instance;
        public static bool IsOpen => Instance != null;

        private TMP_InputField? _searchInput;
        private Transform? _contentRoot;
        private GameObject? _buttonTemplate;
        private bool _hasPopulated;

        private Transform? _modifierContentRoot;
        private GameObject? _modifierButtonTemplate;
        private bool _hasPopulatedModifiers;
        
        private Transform _infoContent;
        private TextMeshProUGUI _roleLongDescription;
        
        private enum ModifierCategory
        {
            Universal,
            Crewmate,
            Impostor,
            Neutral,
            Coven
        }

        private void OnEnable()
        {
            if (PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt();
            }
        }

        private void OnDisable()
        {
            if (PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.moveable = true;
            }
        }

        private Color GetFactionColor(string faction)
        {
            return faction switch
            {
                "Crewmate" => LaunchpadPalette.Crewmate,
                "Impostor" => LaunchpadPalette.Impostor,
                "Neutral" => LaunchpadPalette.Neutral,
                "Coven" => LaunchpadPalette.Coven,
                _ => Color.black
            };
        }

        private ModifierCategory GetModifierCategoryType(GameModifier modifier)
        {
            if (modifier is LPModifier)
                return ModifierCategory.Universal;

            var type = modifier.GetType().Namespace;

            if (type == null)
                return ModifierCategory.Universal;

            if (type.Contains(".Crewmate"))
                return ModifierCategory.Crewmate;

            if (type.Contains(".Impostor"))
                return ModifierCategory.Impostor;

            if (type.Contains(".Neutral"))
                return ModifierCategory.Neutral;

            if (type.Contains(".Coven"))
                return ModifierCategory.Coven;

            return ModifierCategory.Universal;
        }

        private Color GetModifierCategoryColor(GameModifier modifier)
        {
            return GetModifierCategoryType(modifier) switch
            {
                ModifierCategory.Universal => new Color(0f, 1f, 0.027f),
                ModifierCategory.Crewmate => LaunchpadPalette.Crewmate,
                ModifierCategory.Impostor => LaunchpadPalette.Impostor,
                ModifierCategory.Neutral => LaunchpadPalette.Neutral,
                ModifierCategory.Coven => LaunchpadPalette.Coven,
                _ => Color.black
            };
        }

        private string GetModifierCategory(GameModifier modifier)
        {
            return GetModifierCategoryType(modifier) switch
            {
                ModifierCategory.Universal => "Universal",
                ModifierCategory.Crewmate => "Crewmate",
                ModifierCategory.Impostor => "Impostor",
                ModifierCategory.Neutral => "Neutral",
                ModifierCategory.Coven => "Coven",
                _ => "Unknown"
            };
        }

        private void Awake()
        {
            Instance = this;

            _searchInput = GetComponentInChildren<TMP_InputField>(true);
            if (_searchInput != null)
            {
                _searchInput.onValueChanged.AddListener(
                    (UnityAction<string>)OnSearchChanged
                );
            }

            _contentRoot = FindChildRecursive(transform, "RoleContent");
            if (_contentRoot == null)
            {
                Debug.LogError("[Wiki] Could not find RoleContent!");
                return;
            }

            for (int i = 0; i < _contentRoot.childCount; i++)
            {
                var child = _contentRoot.GetChild(i);
                if (child != null && child.gameObject.activeSelf)
                {
                    _buttonTemplate = child.gameObject;
                    break;
                }
            }

            if (_buttonTemplate == null)
            {
                Debug.LogError("[Wiki] No RoleButton template found!");
                return;
            }

            _buttonTemplate.SetActive(false);

            _modifierContentRoot = FindChildRecursive(transform, "ModifierContent");

            if (_modifierContentRoot == null)
            {
                Debug.LogError("[Wiki] Could not find ModifierContent!");
                return;
            }

            for (int i = 0; i < _modifierContentRoot.childCount; i++)
            {
                var child = _modifierContentRoot.GetChild(i);
                if (child != null && child.gameObject.activeSelf)
                {
                    _modifierButtonTemplate = child.gameObject;
                    break;
                }
            }

            if (_modifierButtonTemplate == null)
            {
                Debug.LogError("[Wiki] No ModifierButton template found!");
                return;
            }

            _modifierButtonTemplate.SetActive(false);
            
            _infoContent = FindChildRecursive(transform, "RoleInfo");
            if (_infoContent != null)
            {
                _roleLongDescription = FindChildRecursive(_infoContent, "Description")?.GetComponent<TextMeshProUGUI>();
            }

            var exitBtn = FindChildRecursive(transform, "X")
                ?.GetComponent<PassiveButton>();

            if (exitBtn != null)
            {
                exitBtn.OnClick.AddListener((UnityAction)Close);
            }
        }

        private void Update()
        {
            if (_hasPopulated)
                return;

            if (RoleManager.Instance == null)
                return;

            if (RoleManager.Instance.AllRoles == null)
                return;

            PopulateRoles();
            _hasPopulated = true;

            if (!_hasPopulatedModifiers &&
                ModifierManager.Modifiers != null &&
                ModifierManager.Modifiers.Count > 0)
            {
                PopulateModifiers();
                _hasPopulatedModifiers = true;
            }

            if (_searchInput != null)
            {
                OnSearchChanged(_searchInput.text);
                OnModifierSearchChanged(_searchInput.text);
            }
        }

        private Transform? FindChildRecursive(Transform parent, string name)
        {
            foreach (var t in parent.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == name)
                    return t;
            }
            return null;
        }
        
        [HideFromIl2Cpp]
public void SelectRole(ICustomRole role)
{
    if (role == null || _infoContent == null) return;

    // --- Update Role Description ---
    if (_roleLongDescription != null)
    {
        string baseDescription =
            string.IsNullOrEmpty(role.RoleLongDescription)
                ? "No description available."
                : role.RoleLongDescription;

        _roleLongDescription.text =
            baseDescription;
    }

    // --- Find the Info container inside RoleInfo ---
    var infoContainer = FindChildRecursive(_infoContent, "Info");
    if (infoContainer == null)
    {
        Debug.LogWarning("[Wiki] Info container not found in RoleInfo!");
        return;
    }

    // --- Update Role Name ---
    var nameText = infoContainer
        .GetComponentsInChildren<TextMeshProUGUI>(true)
        .FirstOrDefault(t => t.name.Equals("Name", StringComparison.OrdinalIgnoreCase));
    if (nameText != null)
        nameText.text = role.RoleName;
    else
        Debug.LogWarning("[Wiki] Role Name text not found in Info!");

    // --- Update Role Icon ---
    var iconImage = infoContainer
        .GetComponentsInChildren<Image>(true)
        .FirstOrDefault(i => i.name.Equals("Icon", StringComparison.OrdinalIgnoreCase));
    if (iconImage != null && role.Configuration.Icon != null)
    {
        var sprite = role.Configuration.Icon.LoadAsset();
        if (sprite != null)
            iconImage.sprite = sprite;
        else
            Debug.LogWarning($"[Wiki] Icon sprite is null for {role.RoleName}");
    }
    else
    {
        Debug.LogWarning("[Wiki] Icon Image not found in Info!");
    }

    // --- Update Faction Text ---
    var factionText = infoContainer
        .GetComponentsInChildren<TextMeshProUGUI>(true)
        .FirstOrDefault(t => t.name.Equals("FactionText", StringComparison.OrdinalIgnoreCase));
    if (factionText != null)
    {
        string faction = role switch
        {
            ICrewmateRole => "Crewmate",
            IImpostorRole => "Impostor",
            INeutralRole => "Neutral",
            ICovenRole => "Coven",
            _ => "Unknown"
        };

        Color col = GetFactionColor(faction);
        string hexColor = ColorUtility.ToHtmlStringRGB(col);
        factionText.text = $"<color=#{hexColor}>{faction}</color>";
    }
    else
    {
        Debug.LogWarning("[Wiki] FactionText not found in Info!");
    }

    // --- Clean up any cloned children if present ---
    for (int i = _infoContent.childCount - 1; i >= 0; i--)
    {
        var child = _infoContent.GetChild(i).gameObject;
        if (child == _roleLongDescription?.gameObject) continue;
        if (child == nameText?.gameObject) continue;
        if (child == iconImage?.gameObject) continue;
        if (child == factionText?.gameObject) continue;
        if (child.name.Contains("(Clone)")) Destroy(child);
    }
}
        
        [HideFromIl2Cpp]
        public void OnRoleButtonClicked(int roleIndex)
        {
            if (RoleManager.Instance.AllRoles == null) return;
            if (roleIndex < 0 || roleIndex >= RoleManager.Instance.AllRoles.Count) return;

            var role = RoleManager.Instance.AllRoles[roleIndex] as ICustomRole;
            SelectRole(role);
        }

        [HideFromIl2Cpp]
        public void PopulateRoles()
        {
            if (_contentRoot == null || _buttonTemplate == null)
                return;

            for (int i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                var child = _contentRoot.GetChild(i).gameObject;
                if (child != _buttonTemplate)
                    Destroy(child);
            }

            _buttonTemplate.SetActive(false);

            var roles = RoleManager.Instance.AllRoles;
            for (int i = 0; i < roles.Count; i++)
            {
                var role = roles[i];
                if (role == null) continue;

                string faction;
                if (role is ICrewmateRole) faction = "Crewmate";
                else if (role is IImpostorRole) faction = "Impostor";
                else if (role is INeutralRole) faction = "Neutral";
                else if (role is ICovenRole) faction = "Coven";
                else continue;

                var button = Instantiate(_buttonTemplate, _contentRoot);
                button.SetActive(true);
                button.transform.localScale = Vector3.one;

                string roleName =
                    TranslationController.Instance.GetString(role.StringName);

                button.name = roleName;

                var nameText =
                    button.GetComponentInChildren<TextMeshProUGUI>(true);

                if (nameText != null)
                    nameText.text = roleName;

                var factionText =
                    button.transform.Find("Faction/RoleFactionText")
                    ?.GetComponent<TextMeshProUGUI>();

                if (factionText != null)
                    factionText.text = faction;

                var icon =
                    button.transform.Find("RoleIcon")
                    ?.GetComponent<Image>();

                var roleBehaviour = role as ICustomRole;
                if (icon != null &&
                    roleBehaviour?.Configuration.Icon != null)
                {
                    var sprite =
                        roleBehaviour.Configuration.Icon.LoadAsset();

                    if (sprite != null)
                        icon.sprite = sprite;
                }

                var tintImage = button.transform.Find("Tint")?.GetComponent<Image>();
                if (tintImage != null)
                {
                    tintImage.color = GetFactionColor(faction);
                }
                
                int index = i;
                var btnComp = button.GetComponent<Button>();
                if (btnComp != null)
                {
                    btnComp.onClick.AddListener(new Action(delegate { OnRoleButtonClicked(index); }));
                }
            }

            Debug.Log("[Wiki] Role buttons populated successfully");
        }

        [HideFromIl2Cpp]
        public void PopulateModifiers()
        {
            if (_modifierContentRoot == null || _modifierButtonTemplate == null)
                return;

            for (int i = _modifierContentRoot.childCount - 1; i >= 0; i--)
            {
                var child = _modifierContentRoot.GetChild(i).gameObject;
                if (child != _modifierButtonTemplate)
                    Destroy(child);
            }

            _modifierButtonTemplate.SetActive(false);

            foreach (var modifier in ModifierManager.Modifiers)
            {
                if (modifier is not GameModifier gameModifier)
                    continue;

                var button = Instantiate(
                    _modifierButtonTemplate,
                    _modifierContentRoot
                );

                button.SetActive(true);
                button.transform.localScale = Vector3.one;

                var nameText = button.transform
                    .Find("ModifierName")
                    ?.GetComponent<TextMeshProUGUI>();

                if (nameText != null)
                {
                    nameText.richText = true;
                    nameText.text = gameModifier.ModifierName;
                }

                button.name = gameModifier.ModifierName;

                var categoryText = button.transform
                    .Find("Faction/ModifierFactionText")
                    ?.GetComponent<TextMeshProUGUI>();

                if (categoryText != null)
                    categoryText.text = GetModifierCategory(gameModifier);

                var icon = button.transform
                    .Find("ModifierIcon")
                    ?.GetComponent<Image>();

                if (icon != null && gameModifier.ModifierIcon != null)
                {
                    var sprite = gameModifier.ModifierIcon.LoadAsset();
                    if (sprite != null)
                        icon.sprite = sprite;
                }

                var tint = button.transform
                    .Find("Tint")
                    ?.GetComponent<Image>();

                if (tint != null)
                    tint.color = GetModifierCategoryColor(gameModifier);
            }

            Debug.Log("[Wiki] Modifier buttons populated successfully");
        }

        [HideFromIl2Cpp]
        public void OnSearchChanged(string query)
        {
            if (_contentRoot == null || _buttonTemplate == null) return;

            string clean = query.Trim().ToLowerInvariant();

            for (int i = 0; i < _contentRoot.childCount; i++)
            {
                var child = _contentRoot.GetChild(i);
                if (child.gameObject == _buttonTemplate)
                    continue;

                var nameText = child
                    .Find("RoleName")
                    ?.GetComponent<TextMeshProUGUI>();

                if (nameText == null)
                    continue;

                bool match =
                    string.IsNullOrEmpty(clean) ||
                    nameText.text.ToLowerInvariant().Contains(clean);

                child.gameObject.SetActive(match);
            }
        }

        [HideFromIl2Cpp]
        public void OnModifierSearchChanged(string query)
        {
            if (_modifierContentRoot == null || _modifierButtonTemplate == null) return;

            string clean = query.Trim().ToLowerInvariant();

            for (int i = 0; i < _modifierContentRoot.childCount; i++)
            {
                var child = _modifierContentRoot.GetChild(i);
                if (child.gameObject == _modifierButtonTemplate)
                    continue;

                var nameText = child
                    .Find("ModifierName")
                    ?.GetComponent<TextMeshProUGUI>();

                if (nameText == null)
                    continue;

                bool match =
                    string.IsNullOrEmpty(clean) ||
                    nameText.text.ToLowerInvariant().Contains(clean);

                child.gameObject.SetActive(match);
            }
        }

        public void Close()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
}
