using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    public UpgradeType upgradeType;

    // Example apply method — call this from UpgradeButton or manager
    public void ApplyUpgrade(PlayerController player/*, SnakeScript snake*/)
    {
        switch (upgradeType)
        {
            case UpgradeType.DoubleJump:
                var doubleJumpHandler = player.GetComponentInChildren<JumpHandler>();
                if (doubleJumpHandler != null)
                {
                    doubleJumpHandler._maxJumps += 1;
                }
                break;
            case UpgradeType.Dash:
                var dashHandler = player.GetComponentInChildren<DashHandler>();
                if (dashHandler != null)
                {
                    dashHandler.MaxConsecutiveDashes += 1;
                }
                break;
            case UpgradeType.Health:
                //snake.DecreaseVelocity(-1000.0f);

                break;  

        }
    }

}

public enum UpgradeType { Health, Sprint, DoubleJump, Dash }

