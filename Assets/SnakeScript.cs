using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

public class SnakeScript : MonoBehaviour
{   
    private GameObject player;
    private CameraScript camera;
    public SnakeLogic snakeLogic;
    public ArrowScript arrowScript;

    [Header ("Speed Settings")]
    public float velocity = 0.05f;
    public float increaseModifier = 1.01f;
    public float increaseInterval = 1f;
    public float increaseValue = 0.01f;

    private float increaseTimer = 0f;
    private float baseVelocity;
    private bool isStopping = false;
    private float stopTimer = 0f;
    private float previousVelocity = 0f;

    [Header ("Camera Settings")]
    public float CameraActivationDistance = 10.0f;
    private bool cameraTriggered = false;

    [Header ("Arrow Settings")]
    public float hideArrowDistance = 3.0f;
    public float blinkArrowDistance = 7.0f;

    private float distance;
    private float startDistance;
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        camera = Camera.main.GetComponent<CameraScript>();
        startDistance = DistanceToPlayer();
        baseVelocity = velocity;
    }

    private void Update()
    {
        // get the distance to player
        distance = DistanceToPlayer();

        // if the snake is getting closer move the camera
        TriggerBossEvent();

        // handle arrow visibility
        TriggerArrow();

        // Increase Speed
        if (!isStopping)
        {
            increaseTimer += Time.deltaTime;
            if (increaseTimer >= increaseInterval)
            {
                IncreaseVelocity();
                increaseTimer = 0f;
            }
        }

    }

   

    void FixedUpdate()
    {   
        // move the snake
        Move();
    }

    public float DistanceToPlayer()
    {
        return player.transform.position.x - transform.position.x;
    }

    public void SetDistanceToPlayer(float distance)
    {
        transform.position = new Vector3(player.transform.position.x - distance, transform.position.y, transform.position.z);
    }

    void TriggerBossEvent()
    {
        if (distance < CameraActivationDistance && !cameraTriggered)
        {
            camera.bossEvent = true;
            cameraTriggered = true;
        }
        if (distance > CameraActivationDistance && cameraTriggered)
        {
            camera.bossEvent = false;
            cameraTriggered = false;
        }
    }

    void TriggerArrow()
    {
        if (isStopping)
        {
            arrowScript.Hide();
            return;
        }

        if (distance < hideArrowDistance)
        {
            arrowScript.Hide();
            return;
        } 
        else if (distance < blinkArrowDistance && distance >= hideArrowDistance)
        {
            arrowScript.Show();
            arrowScript.ChangeSize(distance);
            arrowScript.StartBlinking();
            return;
        }
        else
        {
            arrowScript.Show();
            arrowScript.ChangeSize(distance);
            arrowScript.StopBlinking();
            return;

        }
    }

    private void Move()
    {
        Vector3 move = new Vector3(isStopping ? transform.position.x : transform.position.x + velocity, player.transform.position.y + 4, transform.position.z);
        transform.position = move;
    }

    public void IncreaseVelocity()
    {
        velocity *= increaseModifier;
    }

    public void DecreaseVelocity(float amount)
    {
        if (!isStopping)
        {
            if (velocity - amount < baseVelocity) velocity = baseVelocity;
            else velocity -= amount;
        }
        else
        {
            if (previousVelocity - amount < baseVelocity) previousVelocity = baseVelocity;
            else previousVelocity -= amount;
        }

    }

    public void resetPosition()
    {
        transform.position = new Vector3(player.transform.position.x - startDistance, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            snakeLogic.PlayerDefeated();
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}

