using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLevelGenerator : MonoBehaviour
{
    public int chunksBeforePlatformer = 3;
    public int endlessRunnerCount = 0;

    public Camera mainCamera;
    public Transform chunkParent;
    public GameObject[] chunkTemplates;
    public GameObject[] platfromChunkTemplates;

    private List<GameObject> _viableEndlessChunkTemplates = new List<GameObject>();
    private List<GameObject> _viablePlatfromChunkTemplates = new List<GameObject>();
    public GameObject startChunk;

    [Header("Ustawienia")]
    public float spawnBuffer = 10f;
    public float despawnBuffer = 10f;

    private List<Chunk> activeChunks = new List<Chunk>();
    private float rightmostX = 0f;

    private PlayerSkills _playerSkills;

    private GameObject _lastEndlessChunk = null;
    private GameObject _lastPlatformChunk = null;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerSkills = playerObject.GetComponent<PlayerSkills>();
        }
        if (_playerSkills == null)
        {
            Debug.LogError("PlayerSkills nie znaleziono na graczu. Generator nie bedzie dzialal poprawnie.");
        }

        UpdateViableChunks();
        SpawnNextChunk(Vector3.zero, startChunk);
    }

    void Update()
    {
        float cameraRight = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect + spawnBuffer;
        while (rightmostX < cameraRight)
        {
            GameObject nextChunk = null;

            if (endlessRunnerCount >= chunksBeforePlatformer)
            {
                if (_viablePlatfromChunkTemplates.Count > 0)
                {
                    nextChunk = GetNextChunk(_viablePlatfromChunkTemplates, ref _lastPlatformChunk);
                    if (nextChunk != null)
                    {
                        endlessRunnerCount = 0;
                    }
                }

                // Jesli nie udalo sie wylosowac platformowego chunka (bo np. lista jest pusta)
                if (nextChunk == null)
                {
                    // Awaryjnie wracamy do endless runnera
                    nextChunk = GetNextChunk(_viableEndlessChunkTemplates, ref _lastEndlessChunk);
                }
            }
            else
            {
                nextChunk = GetNextChunk(_viableEndlessChunkTemplates, ref _lastEndlessChunk);
                if (nextChunk != null)
                {
                    endlessRunnerCount++;
                }
            }

            if (nextChunk != null)
            {
                SpawnNextChunk(Vector3.zero, nextChunk);
            }
            else
            {
                Debug.LogError("Brak jakichkolwiek dostepnych chunkow do wygenerowania");
                rightmostX = cameraRight;
            }
        }

        float cameraLeft = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect - despawnBuffer;
        if (activeChunks.Count > 0 && activeChunks[0].Exit.position.x < cameraLeft)
        {
            DespawnFirstChunk();
        }
    }

    private GameObject GetNextChunk(List<GameObject> chunks, ref GameObject lastChunk)
    {
        // Jesli jest dostepne wiecej niz 2 unikalne chunku, to filtrujemy tak zeby nie powtarzac ostatnio uzytego.
        if (chunks.Count > 1)
        {
            List<GameObject> filteredList = new List<GameObject>(chunks);
            if (lastChunk != null && filteredList.Contains(lastChunk))
            {
                filteredList.Remove(lastChunk);
            }

            GameObject selectedChunk = filteredList[Random.Range(0, filteredList.Count)];
            lastChunk = selectedChunk;
            return selectedChunk;
        }
        // Jesli jest tylko 1 opcja, to musimy ja powtorzyc.
        else if (chunks.Count == 1)
        {
            lastChunk = chunks[0];
            return chunks[0];
        }
        return null;
    }

    void SpawnNextChunk(Vector3 spawnPosition, GameObject prefab)
    {

        GameObject go = Instantiate(prefab, chunkParent);
        Chunk chunk = go.GetComponent<Chunk>();

        if (chunk.isFlippable && Random.value < 0.5f)
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

    public void UpdateViableChunks()
    {
        _viableEndlessChunkTemplates.Clear();
        _viablePlatfromChunkTemplates.Clear();

        foreach (GameObject prefab in chunkTemplates)
        {
            Chunk chunk = prefab.GetComponent<Chunk>();
            if (chunk == null) continue;

            if (IsChunkViable(chunk))
            {
                _viableEndlessChunkTemplates.Add(prefab);
            }
        }

        foreach (GameObject prefab in platfromChunkTemplates)
        {
            Chunk chunk = prefab.GetComponent<Chunk>();
            if (chunk == null) continue;

            if (IsChunkViable(chunk))
            {
                _viablePlatfromChunkTemplates.Add(prefab);
            }
        }

        if (_viableEndlessChunkTemplates.Count == 0)
        {
            Debug.LogError("Brak dostepnych endless runner chunkow.");
        }
    }

    private bool IsChunkViable(Chunk chunk)
    {
        if (_playerSkills == null) return true;

        // Sprawdzenie poziomu trudnosci
        if (chunk.difficultyLevel > _playerSkills.CurrentDifficulty)
        {
            return false;
        }

        // Sprawdzenie wall jump
        if (chunk.wallJumpingRequired && _playerSkills.SameWallJumpMaxAmount == 0)
        {
            return false;
        }

        // Sprawdzenie liczby odbic od sciany w wall jump
        if (chunk.howManySameWallMaxJumps > _playerSkills.SameWallJumpMaxAmount)
        {
            return false;
        }

        // Sprawdzenie liczby skokow
        if (chunk.howManyJumps > _playerSkills.PlayerJumps)
        {
            return false;
        }

        // Sprawdzenie liczby dashy
        if (chunk.howManyDashes > _playerSkills.PlayerDashes)
        {
            return false;
        }

        return true;
    }
}
