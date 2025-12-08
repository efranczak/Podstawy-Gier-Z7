using UnityEngine;

public class CameraScript : MonoBehaviour
{   
    private Transform player;
    private int offsetDirection = 1;

    public float smoothSpeed = 0.1f;
    [ Range(0.0f, 0.4f) ]  public float offsetX;
    public bool bossEvent = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {   

        UpdateCamera();
    }

    float lerp(float a, float b, float t)
    {
        return a + (b - a) * t; 
    }

    private void UpdateCamera()
    {
        // if boss is getting closer, move camera to right
        if (bossEvent) offsetDirection = -1;
        else offsetDirection = 1;

        // update camera position
        float targetX = player.position.x;
        transform.position = new Vector3(lerp(transform.position.x + offsetDirection*offsetX, targetX, smoothSpeed), transform.position.y, transform.position.z);

    }

    
}
