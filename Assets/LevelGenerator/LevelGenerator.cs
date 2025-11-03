using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Camera mainCamera;
    public Transform chunkParent;
    public GameObject[] chunkTemplates;

    [Header("Ustawienia")]
    public float spawnBuffer = 10f;
    public float despawnBuffer = 10f;

    private List<Chunk> activeChunks = new List<Chunk>();
    private float rightmostX = 0f;

    void Start()
    {
        SpawnNextChunk(Vector3.zero);
    }

    void Update()
    {
        float cameraRight = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect + spawnBuffer;
        while (rightmostX < cameraRight)
        {
            SpawnNextChunk(Vector3.zero);
        }

        float cameraLeft = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect - despawnBuffer;
        if (activeChunks.Count > 0 && activeChunks[0].Exit.position.x < cameraLeft)
        {
            DespawnFirstChunk();
        }
    }

    void SpawnNextChunk(Vector3 spawnPosition)
    {
        GameObject prefab = chunkTemplates[Random.Range(0, chunkTemplates.Length)];
        GameObject go = Instantiate(prefab, chunkParent);
        Chunk chunk = go.GetComponent<Chunk>();

        if (Random.value < 0.5f)
        {
            Vector3 scale = go.transform.localScale;
            scale.x *= -1;
            go.transform.localScale = scale;
        }

        if (go.transform.localScale.x < 0)
        {
            Transform tmp = chunk.Entry;
            chunk.Entry = chunk.Exit;
            chunk.Exit = tmp;
        }

        if (activeChunks.Count > 0)
        {
            Chunk prev = activeChunks[activeChunks.Count - 1];
            Vector3 delta = prev.Exit.position - chunk.Entry.position;
            go.transform.position += delta;
        }
        else
        {
            go.transform.position = spawnPosition;
        }

        activeChunks.Add(chunk);
        rightmostX = chunk.Exit.position.x;
    }

    void DespawnFirstChunk()
    {
        Chunk first = activeChunks[0];
        activeChunks.RemoveAt(0);
        Destroy(first.gameObject);
    }
}
