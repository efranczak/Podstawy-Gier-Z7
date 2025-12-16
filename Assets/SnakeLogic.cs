using UnityEngine;

public class SnakeLogic : MonoBehaviour
{

    [Header("Snake Hunger")]
    public int maxHunger = 4;
    public int currentHunger;

    public HealthBar hungerBar;
    public ApplesUI appleUI;
    public GameOverScript gameOverScript;
    public SnakeScript snakeScript;
    public UpgradePanelUI upgradePanelUI;

    void Start()
    {
        currentHunger = maxHunger;
        hungerBar.SetMaxHealth(maxHunger);
        appleUI.CreateApples(maxHunger);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SnakeDefeated();
        }
    }

    public void DecreaseHunger(int amount)
    {
        currentHunger -= amount;
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            appleUI.ResetApples();
            SnakeDefeated();
        }
        else
        {
            appleUI.setApples(maxHunger - currentHunger);
        }   
    }

    void SnakeDefeated()
    {   
        upgradePanelUI.ShowUpgradeSelection();
        // snakeScript.resetPosition();
    }


    public void PlayerDefeated()
    {   
        snakeScript.velocity = 0f;
        gameOverScript.TriggerGameOver();
    }
}
