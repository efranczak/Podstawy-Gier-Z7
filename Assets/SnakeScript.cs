using TreeEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class SnakeScript : MonoBehaviour
{   
    private GameObject player;
    private CameraScript camera;

    public float velocity = 0.05f;

    public float CameraActivationDistance = 10.0f;
    private bool cameraTriggered = false;

    private float distance;
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        camera = Camera.main.GetComponent<CameraScript>();
    }

    private void Update()
    {
        // get the distance to player
        distance = DistanceToPlayer();

        // if the snake is getting closer move the camera
        TriggerBossEvent();
        
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

    private void Move()
    {
        Vector3 move = new Vector3(transform.position.x + velocity, transform.position.y, transform.position.z);
        transform.position = move;
    }
}
