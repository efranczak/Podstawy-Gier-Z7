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
        PlayerSkills playerSkills = player.GetComponent<PlayerSkills>();

        PlatformLevelGenerator generator = FindAnyObjectByType<PlatformLevelGenerator>();

        switch (upgradeType)
        {
            case UpgradeType.DoubleJump:
                var doubleJumpHandler = player.GetComponentInChildren<JumpHandler>();
                if (doubleJumpHandler != null)
                {
                    playerSkills.SetMaxJumps(playerSkills.PlayerJumps + 1);
                    generator.UpdateViableChunks();
                }
                break;
            case UpgradeType.Dash:
                var dashHandler = player.GetComponentInChildren<DashHandler>();
                if (dashHandler != null)
                {
                    playerSkills.SetMaxDashes(playerSkills.PlayerDashes + 1);
                    generator.UpdateViableChunks();
                }
                break;
            case UpgradeType.Health:
                //snake.DecreaseVelocity(-1000.0f);
                break;
            case UpgradeType.WallJump:
                    playerSkills.SetWallJumpStatus(true);
                break;
        }
    }

}

public enum UpgradeType { Health, Sprint, DoubleJump, Dash, WallJump }

