using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
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
using MiraAPI.Utilities;
using TORWL.Utilities;

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
        
        private Transform? _infoContent;
        private TextMeshProUGUI? _roleLongDescription;
        
        private Transform? _modifierInfoContent;
        private TextMeshProUGUI? _modifierDescription;
        
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

            _infoContent = FindChildRecursive(transform, "RoleInfo")!;
            if (_infoContent != null)
            {
                _roleLongDescription = FindChildRecursive(_infoContent, "Description")?.GetComponent<TextMeshProUGUI>()!;
            }
            
            _modifierInfoContent = FindChildRecursive(transform, "ModifierInfo")!;
            if (_modifierInfoContent != null)
            {
                _modifierDescription = FindChildRecursive(_modifierInfoContent, "Description")?.GetComponent<TextMeshProUGUI>()!;
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
        public void SelectRole(ICustomRole? role)
        {
            if (role == null || _infoContent == null) return;

            if (_roleLongDescription != null)
            {
                string baseDescription =
                    string.IsNullOrEmpty(role.RoleLongDescription)
                        ? $"{role.RoleName} has no set description.\nIt is either missing, does not exist or is a different type of role."
                        : role.RoleLongDescription;

                _roleLongDescription.text = baseDescription;
            }

            var infoContainer = FindChildRecursive(_infoContent, "Info");
            if (infoContainer == null)
            {
                Debug.LogWarning("[Wiki] Info container not found in RoleInfo!");
                return;
            }

            var nameText = infoContainer
                .GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(t => t.name.Equals("Name", StringComparison.OrdinalIgnoreCase));
            if (nameText != null)
                nameText.text = role.RoleName;

            var iconImage = infoContainer
                .GetComponentsInChildren<Image>(true)
                .FirstOrDefault(i => i.name.Equals("Icon", StringComparison.OrdinalIgnoreCase));
            if (iconImage != null && role.Configuration.Icon != null)
            {
                var sprite = role.Configuration.Icon.LoadAsset();
                if (sprite != null)
                    iconImage.sprite = sprite;
            }

            var factionText = infoContainer
                .GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(t => t.name.Equals("FactionText", StringComparison.OrdinalIgnoreCase));

            if (factionText != null)
            {
                factionText.richText = true;

                factionText.text = role switch
                {
                    ICrewmateRole crewmate => Utils.GetCrewmateFactionDisplay(crewmate),
                    IImpostorRole impostor => Utils.GetImpostorFactionDisplay(impostor),
                    INeutralRole neutral   => Utils.GetNeutralFactionDisplay(neutral),
                    ICovenRole coven       => Utils.GetCovenFactionDisplay(coven),
                    _ => "Unknown"
                };
            }

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
        public void SelectModifier(GameModifier? mod)
        {
            if (mod == null || _modifierInfoContent == null) return;

            if (_modifierDescription != null)
            {
                string baseDescription =
                    string.IsNullOrEmpty(mod.GetDescription())
                        ? $"{mod.ModifierName} has no set description."
                        : mod.GetDescription();

                _modifierDescription.text = baseDescription;
            }
            
            var modInfoContainer = FindChildRecursive(_modifierInfoContent, "Info");
            if (modInfoContainer == null) return;

            var modNameText = modInfoContainer
                .GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(t => t.name.Equals("Name", StringComparison.OrdinalIgnoreCase));
            if (modNameText != null)
                modNameText.text = $"<color=#FFFFFF>{mod.ModifierName}</color>";

            var modIconImage = modInfoContainer
                .GetComponentsInChildren<Image>(true)
                .FirstOrDefault(i => i.name.Equals("Icon", StringComparison.OrdinalIgnoreCase));
            if (modIconImage != null && mod.ModifierIcon != null)
            {
                var sprite = mod.ModifierIcon.LoadAsset();
                if (sprite != null) modIconImage.sprite = sprite;
            }
            
            var modCategory = GetModifierCategoryType(mod);
            var modFactionText = modInfoContainer
                .GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(t => t.name.Equals("FactionText", StringComparison.OrdinalIgnoreCase));
            
            if (modFactionText != null)
            {
                string faction = GetModifierCategory(mod);
                Color col = GetModifierCategoryColor(mod);
                string hexColor = ColorUtility.ToHtmlStringRGB(col);
                modFactionText.text = $"<color=#{hexColor}>{faction}</color>";
            }

            for (int i = _modifierInfoContent.childCount - 1; i >= 0; i--)
            {
                var child = _modifierInfoContent.GetChild(i).gameObject;
                if (child == _modifierDescription?.gameObject) continue;
                if (child == modNameText?.gameObject) continue;
                if (child == modIconImage?.gameObject) continue;
                if (child == modFactionText?.gameObject) continue;
                if (child.name.Contains("(Clone)")) Destroy(child);
            }
        }

        [HideFromIl2Cpp]
        public void OnRoleButtonClicked(int roleIndex)
        {
            if (RoleManager.Instance.AllRoles == null) return;
            if (roleIndex < 0 || roleIndex >= RoleManager.Instance.AllRoles.Count) return;

            var role = RoleManager.Instance.AllRoles[roleIndex] as ICustomRole;
            // Fix CS8604: Ensure role isn't null
            if (role != null) SelectRole(role);
        }
        
        [HideFromIl2Cpp]
        public void OnModifierButtonClicked(int modifierIndex)
        {
            if (ModifierManager.Modifiers == null) return;
            if (modifierIndex < 0 || modifierIndex >= ModifierManager.Modifiers.Count) return;

            var mod = ModifierManager.Modifiers[modifierIndex] as GameModifier;
            // Fix CS8604: Ensure mod isn't null
            if (mod != null) SelectModifier(mod);
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

                string? factionDisplay = role switch
                {
                    ICrewmateRole crewmate => Utils.GetCrewmateFactionDisplay(crewmate),
                    IImpostorRole impostor => Utils.GetImpostorFactionDisplay(impostor),
                    INeutralRole neutral   => Utils.GetNeutralFactionDisplay(neutral),
                    ICovenRole coven       => Utils.GetCovenFactionDisplay(coven),
                    _ => null
                };

                if (factionDisplay == null)
                    continue;

                var button = Instantiate(_buttonTemplate, _contentRoot);
                button.SetActive(true);
                button.transform.localScale = Vector3.one;

                string roleName = TranslationController.Instance.GetString(role.StringName);
                button.name = roleName;

                var nameText = button.GetComponentInChildren<TextMeshProUGUI>(true);
                if (nameText != null) nameText.text = roleName;

                var factionText = button.transform
                    .Find("Faction/RoleFactionText")
                    ?.GetComponent<TextMeshProUGUI>();

                if (factionText != null)
                {
                    factionText.richText = true;
                    factionText.fontSize = 33f;
                    factionText.text = factionDisplay;
                }

                var tintImage = button.transform.Find("Tint")?.GetComponent<Image>();
                if (tintImage != null)
                {
                    tintImage.color = role switch
                    {
                        ICrewmateRole => LaunchpadPalette.Crewmate,
                        IImpostorRole => LaunchpadPalette.Impostor,
                        INeutralRole => LaunchpadPalette.Neutral,
                        ICovenRole => LaunchpadPalette.Coven,
                        _ => Color.black
                    };
                }

                var icon = button.transform.Find("RoleIcon")?.GetComponent<Image>();
                var roleBehaviour = role as ICustomRole;
                if (icon != null && roleBehaviour?.Configuration.Icon != null)
                {
                    var sprite = roleBehaviour.Configuration.Icon.LoadAsset();
                    if (sprite != null) icon.sprite = sprite;
                }
                
                int index = i;
                var btnComp = button.GetComponent<Button>();
                if (btnComp != null)
                {
                    btnComp.onClick.AddListener(new Action(delegate { OnRoleButtonClicked(index); }));
                }
            }
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

            for (int i = 0; i < ModifierManager.Modifiers.Count; i++)
            {
                var gameModifier = ModifierManager.Modifiers[i] as GameModifier;
                if (gameModifier == null) continue;

                var button = Instantiate(_modifierButtonTemplate, _modifierContentRoot);
                button.SetActive(true);
                button.transform.localScale = Vector3.one;

                var nameText = button.transform.Find("ModifierName")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.richText = true;
                    nameText.text = gameModifier.ModifierName;
                }

                button.name = gameModifier.ModifierName;

                var categoryText = button.transform.Find("Faction/ModifierFactionText")?.GetComponent<TextMeshProUGUI>();
                if (categoryText != null) categoryText.text = GetModifierCategory(gameModifier);

                var icon = button.transform.Find("ModifierIcon")?.GetComponent<Image>();
                if (icon != null && gameModifier.ModifierIcon != null)
                {
                    var sprite = gameModifier.ModifierIcon.LoadAsset();
                    if (sprite != null) icon.sprite = sprite;
                }

                var tint = button.transform.Find("Tint")?.GetComponent<Image>();
                if (tint != null) tint.color = GetModifierCategoryColor(gameModifier);
                
                int index = i;
                var btnComp = button.GetComponent<Button>();
                if (btnComp != null)
                {
                    btnComp.onClick.AddListener(new Action(delegate { OnModifierButtonClicked(index); }));
                }
            }
        }

        [HideFromIl2Cpp]
        public void OnSearchChanged(string query)
        {
            if (_contentRoot == null || _buttonTemplate == null) return;
            string clean = query.Trim().ToLowerInvariant();

            for (int i = 0; i < _contentRoot.childCount; i++)
            {
                var child = _contentRoot.GetChild(i);
                if (child.gameObject == _buttonTemplate) continue;

                var nameText = child.Find("RoleName")?.GetComponent<TextMeshProUGUI>();
                if (nameText == null) continue;

                bool match = string.IsNullOrEmpty(clean) || nameText.text.ToLowerInvariant().Contains(clean);
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
                if (child.gameObject == _modifierButtonTemplate) continue;

                var nameText = child.Find("ModifierName")?.GetComponent<TextMeshProUGUI>();
                if (nameText == null) continue;

                bool match = string.IsNullOrEmpty(clean) || nameText.text.ToLowerInvariant().Contains(clean);
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
