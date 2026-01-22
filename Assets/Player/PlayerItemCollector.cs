using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    public int hungerDecreaseAmount = 10;
    public float speedDecreaseAmount = 2.5f;
    public float distanceIncreaseAmount = 5.0f;
    private SnakeLogic snakeLogic;
    private SnakeScript snakeScript;
    private PlayerAudioManager audioManager;


    void Start()
    {
        snakeLogic = FindAnyObjectByType<SnakeLogic>();
        snakeScript = FindAnyObjectByType<SnakeScript>();
        audioManager = GetComponent<PlayerAudioManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            // Play apple collect sound
            if (audioManager != null)
            {
                audioManager.PlayAppleSound();
            }
            
            // Decrease hunger
            if (snakeLogic != null)
            {
                snakeLogic.DecreaseHunger(hungerDecreaseAmount);
                snakeScript.DecreaseVelocity(speedDecreaseAmount);
                snakeScript.SetDistanceToPlayer(snakeScript.DistanceToPlayer() + distanceIncreaseAmount);
            }
            // Destroy or disable the apple
            Destroy(other.gameObject);
        }
    }
}
