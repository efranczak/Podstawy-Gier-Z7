using UnityEngine;
using UnityEngine.UI;

public class ActiveUpgradeUI : MonoBehaviour
{
    public RawImage icon;
    public UpgradeData currentUpgrade = null;

    private int upgradeCounter = 0;

    public bool SetIcon(UpgradeData upgrade)
    {
        if (upgrade == null) return false;
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


}
