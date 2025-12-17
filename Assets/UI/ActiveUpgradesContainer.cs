using UnityEngine;

public class ActiveUpgradesContainer : MonoBehaviour
{
    public ActiveUpgradeUI[] activeUpgradeUIs;

    public ActiveUpgradeUI[] consumableSlot;

    public void UpdateActiveUpgrades(UpgradeData activeUpgrades)
    {
        for (int i = 0; i < activeUpgradeUIs.Length; i++)
        {
            if (activeUpgradeUIs[i].SetIcon(activeUpgrades)) return;
        }
    }

    public void UpdateConsumableUpgrades(UpgradeData consumableUpgrade)
    {
        for (int i = 0; i < consumableSlot.Length; i++)
        {
            if (consumableSlot[i].SetIcon(consumableUpgrade)) return;
        }
    }

}
