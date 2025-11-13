using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    public int hungerDecreaseAmount = 10;
    public float speedDecreaseAmount = 2.5f;
    private SnakeLogic snakeLogic;
    private SnakeScript snakeScript;


    void Start()
    {
        snakeLogic = FindAnyObjectByType<SnakeLogic>();
        snakeScript = FindAnyObjectByType<SnakeScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            // Decrease hunger
            if (snakeLogic != null)
            {
                snakeLogic.DecreaseHunger(hungerDecreaseAmount);
                snakeScript.DecreaseVelocity(speedDecreaseAmount);
            }
            // Destroy or disable the apple
            Destroy(other.gameObject);
        }
    }
}
