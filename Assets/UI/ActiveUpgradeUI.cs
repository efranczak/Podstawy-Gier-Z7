using UnityEngine;
using UnityEngine.UI;

public class ActiveUpgradeUI : MonoBehaviour
{
    public RawImage icon;
    public UpgradeData currentUpgrade = null;
    public PlayerSkills playerSkills;

    private ConsumableHandler _consumableHandler;
    private ActiveUpgradesText upgradesText;

    private int upgradeCounter = 0;

    private void Awake()
    {
        upgradesText = GetComponentInChildren<ActiveUpgradesText>();
        _consumableHandler = FindAnyObjectByType<ConsumableHandler>();

        if (upgradesText == null)
            Debug.LogError($"[ActiveUpgradeUI] Brak komponentu ActiveUpgradesText w dzieciach obiektu {gameObject.name}!");

        ClearIcon();
    }
    private void Update()
    {
        if (currentUpgrade == null || upgradesText == null) return;

        if (currentUpgrade.category == UpgradeCategory.Permanent)
        {
            if (playerSkills != null)
            {
                switch (currentUpgrade.type)
                {
                    case UpgradeType.DoubleJump: upgradesText.SetText(playerSkills.PlayerJumps.ToString()); break;
                    case UpgradeType.Dash: upgradesText.SetText(playerSkills.PlayerDashes.ToString()); break;
                    case UpgradeType.WallJump: upgradesText.SetText((playerSkills.SameWallJumpMaxAmount + 1).ToString()); break;
                    default: upgradesText.HideText(); break;
                }
            }
        }

        else if (currentUpgrade.category == UpgradeCategory.Consumable)
        {
            if (_consumableHandler != null)
            {
                float timeLeft = _consumableHandler.GetTimeRemaining();

                if (timeLeft > 0)
                {
                    upgradesText.SetText(Mathf.CeilToInt(timeLeft).ToString());
                }
                else
                {
                    upgradesText.HideText();
                }
            }
        }
    }
    public bool SetIcon(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            ClearIcon();
            return true;
        }
        else if (currentUpgrade != upgrade && currentUpgrade != null && upgrade.category != UpgradeCategory.Consumable) return false;
        else
        { 
        icon.texture = upgrade.iconUI.texture;
        icon.color = Color.white;
        currentUpgrade = upgrade;
        if (upgradesText != null) upgradesText.SetText("");
        upgradeCounter++;
        return true;
        }
    }

    private void ClearIcon()
    {
        icon.texture = null;
        icon.color = new Color(0, 0, 0, 0);
        currentUpgrade = null;
        upgradeCounter = 0;
        if (upgradesText != null) upgradesText.HideText();
    }
}
