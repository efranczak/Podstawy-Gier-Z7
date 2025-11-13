using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    public UpgradeType upgradeType;

    // Example apply method — call this from UpgradeButton or manager
    public void ApplyUpgrade(PlayerController player)
    {
        switch (upgradeType)
        {
            // add implementation for each upgrade type
        }
    }
}

public enum UpgradeType { Health, Sprint, DoubleJump, Dash }

