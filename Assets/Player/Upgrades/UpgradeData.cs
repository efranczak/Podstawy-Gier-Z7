using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    public UpgradeType type;
    public UpgradeCategory category;
    public Sprite iconUI;

    // Example apply method — call this from UpgradeButton or manager
    public void ApplyUpgrade(PlayerController player/*, SnakeScript snake*/)
    {
        PlayerSkills playerSkills = player.GetComponent<PlayerSkills>();

        PlatformLevelGenerator generator = FindAnyObjectByType<PlatformLevelGenerator>();

        if (category == UpgradeCategory.Permanent)
        {
            switch (type)
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
                case UpgradeType.WallJump:
                    {
                        playerSkills.SetSameWallJumpMaxAmount(playerSkills.SameWallJumpMaxAmount + 1);
                        generator.UpdateViableChunks();
                    }
                    break;
            }
        }
        else if (category == UpgradeCategory.Consumable)
        {
            var consumableHandler = player.GetComponentInChildren<ConsumableHandler>();
            if (consumableHandler != null)
            {
                consumableHandler.SetConsumable(this.type);
            }
        }
    }

}

public enum UpgradeType { Health, Sprint, DoubleJump, Dash, WallJump }

public enum UpgradeCategory { Permanent, Consumable }
