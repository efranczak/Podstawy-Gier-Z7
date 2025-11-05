using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    public int hungerDecreaseAmount = 10;    
    private SnakeLogic snakeLogic;


    void Start()
    {
        snakeLogic = FindAnyObjectByType<SnakeLogic>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            // Decrease hunger
            if (snakeLogic != null)
                snakeLogic.DecreaseHunger(hungerDecreaseAmount);

            // Destroy or disable the apple
            Destroy(other.gameObject);
        }
    }
}
