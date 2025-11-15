using UnityEngine;


public class DisplayChunk : MonoBehaviour
    {
        public Transform chunkParent;
        public GameObject[] chunkTemplates;
        public Vector3 spawnPosition = Vector3.zero;

        private int index = 0;
        private GameObject currentChunk;

        void Start()
        {
            LoadChunk(index);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                NextChunk();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Chunk chunk = currentChunk.GetComponent<Chunk>();
                if (chunk != null && chunk.isFlippable && Random.value < 0.5f)
                {
                    Vector3 scale = currentChunk.transform.localScale;
                    scale.x *= -1;
                    currentChunk.transform.localScale = scale;

                    Transform tmp = chunk.Entry;
                    chunk.Entry = chunk.Exit;
                    chunk.Exit = tmp;
                }
            }
        }

        void LoadChunk(int i)
        {
            if (currentChunk != null)
                Destroy(currentChunk);

            currentChunk = Instantiate(chunkTemplates[i], spawnPosition, Quaternion.identity, chunkParent);

            
        }

        void NextChunk()
        {
            index++;
            if (index >= chunkTemplates.Length)
                index = 0;

            LoadChunk(index);
        }
    }



