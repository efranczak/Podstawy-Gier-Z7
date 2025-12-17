using UnityEngine;
using UnityEngine.UI;

public class ActiveUpgradeUI : MonoBehaviour
{
    public RawImage icon;
    public UpgradeData currentUpgrade = null;

    private int upgradeCounter = 0;

    public bool SetIcon(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            ClearIcon();
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
