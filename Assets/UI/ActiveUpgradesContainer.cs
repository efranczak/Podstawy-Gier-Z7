using UnityEngine;

public class ActiveUpgradesContainer : MonoBehaviour
{
    public ActiveUpgradeUI[] activeUpgradeUIs;

    public void UpdateActiveUpgrades(UpgradeData activeUpgrades)
    {
        for (int i = 0; i < activeUpgradeUIs.Length; i++)
        {
            if (activeUpgradeUIs[i].SetIcon(activeUpgrades)) return;
        }
    }

}
