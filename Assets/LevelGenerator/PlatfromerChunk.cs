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
    
    private bool isActive = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
    }

    private void EndPlatfromingSection()
    {
        
        SnakeScript.EndPlatformingSection();
        chunkCamera.Priority = 0;
        
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
