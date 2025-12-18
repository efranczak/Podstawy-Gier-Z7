using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UIElements;
using System.Collections;

public class PlatfromerChunk : Chunk
{

    private SnakeScript SnakeScript;
    public int sectionDuration;
    public PlatfromerApple[] apples;
    public BoxCollider2D entryTrigger;
    public CinemachineCamera chunkCamera;
    public GameObject snakeTail;

    public float tailMoveUpAmount = 0.3f;
    public float tailMoveDuration = 0.25f;
    private Coroutine tailMoveCoroutine;

    public bool newTimeSystem;
    private TimeHandler timeHandler;

    private Transform player;

    private BoxCollider2D playerCameraBoundaryCollider;

    private PlayerSkills _playerSkills;
    private PlatformLevelGenerator _levelGenerator;

    private bool isActive = false;

    private int collectedApples = 0;


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
        if (isActive && collectedApples != GetCollectedApples())
        {
            collectedApples = GetCollectedApples();
            UpdateSnakeTail();
            if (collectedApples >= apples.Length)
            {
                EndPlatfromingSection();
            }
        }
        if (isActive) timeHandler.subtractTime(Time.deltaTime);
    }

    private int GetCollectedApples()
    {
        int collected = 0;
        foreach (PlatfromerApple apple in apples)
        {
            if (apple.isCollected) collected++;
        }
        return collected;
    }

    private void TriggerEntry()
    {   
        if (newTimeSystem) sectionDuration = (int) timeHandler.getTime();
        timeHandler.isSubtractingTime = true;
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
        timeHandler.isSubtractingTime = false;
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

    private void UpdateSnakeTail()
    {
        chunkCamera.GetComponent<CameraShake>().ShakeCamera(3f, 1f);
        if (collectedApples >= apples.Length) Destroy(snakeTail);
        else MoveTailUpSmooth();
    }

    private void MoveTailUpSmooth()
    {
        if (tailMoveCoroutine != null)
            StopCoroutine(tailMoveCoroutine);

        tailMoveCoroutine = StartCoroutine(MoveTailUpCoroutine());
    }

    private IEnumerator MoveTailUpCoroutine()
    {
        if (snakeTail == null) yield break;

        Vector3 startPos = snakeTail.transform.localPosition;
        Vector3 targetPos = startPos + Vector3.up * tailMoveUpAmount;

        float elapsed = 0f;

        while (elapsed < tailMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / tailMoveDuration;

            snakeTail.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        snakeTail.transform.localPosition = targetPos;
    }

}
