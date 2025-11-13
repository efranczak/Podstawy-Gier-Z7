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
            case UpgradeType.DoubleJump:
                var doubleJumpHandler = player.GetComponentInChildren<DoubleJumpHandler>();
                if (doubleJumpHandler != null)
                {
                    doubleJumpHandler.SetEnabled(true);
                }
                break;
        }
    }

}

public enum UpgradeType { Health, Sprint, DoubleJump, Dash }

