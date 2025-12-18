using UnityEngine;
using UnityEngine.UI;

public class ActiveUpgradeUI : MonoBehaviour
{
    public RawImage icon;
    public UpgradeData currentUpgrade = null;
    public PlayerSkills playerSkills;

    private ActiveUpgradesText upgradesText;

    private int upgradeCounter = 0;

    private void Start()
    {
        upgradesText = GetComponentInChildren<ActiveUpgradesText>();
        ClearIcon();
    }


    private void Update()
    {
        if (currentUpgrade == null || upgradesText == null) return;
        switch (currentUpgrade.type)
        {
            case UpgradeType.DoubleJump: upgradesText.SetText(playerSkills.PlayerJumps.ToString()); break;
            case UpgradeType.Dash: upgradesText.SetText(playerSkills.PlayerDashes.ToString()); break;
            case UpgradeType.WallJump: upgradesText.SetText((playerSkills.SameWallJumpMaxAmount+1).ToString()); break;
        }

    }

    public bool SetIcon(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            ClearIcon();
            upgradesText.HideText();
            return true;
        }
        else if (currentUpgrade != upgrade && currentUpgrade != null) return false;
        else
        { 
        icon.texture = upgrade.iconUI.texture;
        icon.color = Color.white;
        currentUpgrade = upgrade;
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
    }


}
