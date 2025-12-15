using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UIElements;

public class PlatfromerChunk : Chunk
{

    private SnakeScript SnakeScript;
    public int sectionDuration;
    public PlatfromerApple apple;
    public BoxCollider2D entryTrigger;
    public CinemachineCamera chunkCamera;
    public GameObject snakeTail;

    public bool newTimeSystem;
    private TimeHandler timeHandler;

    private Transform player;

    private BoxCollider2D playerCameraBoundaryCollider;

    private PlayerSkills _playerSkills;
    private PlatformLevelGenerator _levelGenerator;

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

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerSkills = playerObject.GetComponent<PlayerSkills>();
        }
        _levelGenerator = FindAnyObjectByType<PlatformLevelGenerator>();
        timeHandler = FindAnyObjectByType<TimeHandler>();
    }

    void Update()
    {
        if (isActive && apple.isCollected) EndPlatfromingSection();
        if (isActive) timeHandler.subtractTime(Time.deltaTime);
    }

    private void TriggerEntry()
    {   
        if (newTimeSystem) sectionDuration = (int) timeHandler.getTime();
        SnakeScript.StartPlatfromingSection(sectionDuration, Entry.position.x);
        isActive = true;
        chunkCamera.Priority = 12;
        playerCameraBoundaryCollider.enabled = false;
    }

    private void EndPlatfromingSection()
    {
        if (!isActive) return;

        if (_playerSkills != null)
        {
            _playerSkills.IncreaseDifficulty(1);
        }

        if (_levelGenerator != null)
        {
            _levelGenerator.UpdateViableChunks();
        }

        SnakeScript.EndPlatformingSection(); 
        chunkCamera.Priority = 0;
        Destroy(snakeTail);
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
