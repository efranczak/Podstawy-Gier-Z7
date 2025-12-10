using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Cinemachine;

public class PlatfromerChunk : Chunk
{

    private SnakeScript SnakeScript;
    public int sectionDuration;
    public PlatfromerApple apple;
    public BoxCollider2D entryTrigger;
    public CinemachineCamera chunkCamera;

    private Transform player;

    private BoxCollider2D playerCameraBoundaryCollider;

    private bool isActive = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject boundaryObject = GameObject.FindGameObjectWithTag("CameraBoundary");
        if (boundaryObject != null)
        {
            playerCameraBoundaryCollider = boundaryObject.GetComponent<BoxCollider2D>();
        }
        SnakeScript = FindAnyObjectByType<SnakeScript>();
    }

    void Update()
    {
        if (isActive && apple.isCollected) EndPlatfromingSection();
    }

    private void TriggerEntry()
    { 
        SnakeScript.StartPlatfromingSection(sectionDuration, Entry.position.x);
        isActive = true;
        chunkCamera.Priority = 12;
        playerCameraBoundaryCollider.enabled = false;
    }

    private void EndPlatfromingSection()
    {
        if (!isActive) return;

        SnakeScript.EndPlatformingSection(); 
        chunkCamera.Priority = 0;
        isActive = false;
        playerCameraBoundaryCollider.enabled = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Platforming trigger");
            TriggerEntry();
        }

    }
}
