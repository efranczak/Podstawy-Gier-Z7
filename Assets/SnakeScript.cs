using TreeEditor;
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

    private float increaseTimer = 0f;
    private float baseVelocity;
    private bool isStopping = false;
    private float stopTimer = 0f;
    private float previousVelocity = 0f;

    [Header ("Camera Settings")]
    public float CameraActivationDistance = 10.0f;
    private bool cameraTriggered = false;

    [Header ("Arrow Settings")]
    public float showArrowDistance = 15.0f;
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

    public void StartPlatfromingSection(float stopDuration, float stopX)
    {
        if (isStopping) return;

        isStopping = true;

        previousVelocity = velocity;
        velocity = 0f;


        transform.position = new Vector3(stopX, transform.position.y, transform.position.z);

        CancelInvoke(nameof(EndPlatformingSection)); 
        Invoke(nameof(EndPlatformingSection), stopDuration); 
    }

    public void EndPlatformingSection()
    {
        if (!isStopping) return; 

        CancelInvoke(nameof(EndPlatformingSection)); 

        velocity = previousVelocity;  
        isStopping = false;            
    }



    void FixedUpdate()
    {   
        // move the snake
        Move();
    }

    float DistanceToPlayer()
    {
        return player.transform.position.x - transform.position.x;
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
        if (distance < showArrowDistance && distance > blinkArrowDistance)
        {
            arrowScript.StopBlinking();
            arrowScript.Show();
        }
        else if (distance <= blinkArrowDistance && distance > startDistance)
        {
            arrowScript.StartBlinking();
        }
        else
        {   
            arrowScript.StopBlinking();
            arrowScript.Hide();
        }
    }

    private void Move()
    {
        if (isStopping) return;
        Vector3 move = new Vector3(transform.position.x + velocity, player.transform.position.y + 4, transform.position.z);
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
}

