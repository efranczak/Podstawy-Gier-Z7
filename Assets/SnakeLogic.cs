using UnityEngine;

public class SnakeLogic : MonoBehaviour
{

    [Header("Snake Hunger")]
    public int maxHunger = 100;
    public int currentHunger;

    public HealthBar hungerBar;


    void Start()
    {
        currentHunger = maxHunger;
        hungerBar.SetMaxHealth(maxHunger);
    }

    public void DecreaseHunger(int amount)
    {
        currentHunger -= amount;
        if (currentHunger < 0)
        {
            currentHunger = 0;
            hungerBar.setHealth(currentHunger);
            SnakeDefeated();
        }
        else
        {
            hungerBar.setHealth(currentHunger);
        }   
    }

    void SnakeDefeated()
    {
        // Logic for when the snake is defeated
    }

    public void PlayerDefeated()
    {
        // Logic for when the player is defeated
    }
}
