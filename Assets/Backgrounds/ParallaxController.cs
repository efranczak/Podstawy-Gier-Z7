using UnityEngine;

public class ParallaxController : MonoBehaviour

{
    Transform cam; // Main Camera
    Vector3 camStartPos;
    float distance; // distance between camera and background

    GameObject[] backgrounds;
    Material[] materials;
    float[] backgroundSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backgroundCount = transform.childCount;
        materials = new Material[backgroundCount];
        backgroundSpeed = new float[backgroundCount];
        backgrounds = new GameObject[backgroundCount];

        for (int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackgroundSpeedCalculate(backgroundCount);
    }

    void BackgroundSpeedCalculate (int backgroundCount)
    {
        for (int i = 0; i < backgroundCount; i++) // find the farthest background
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z;
            }
        }

        for (int i = 0; i < backgroundCount; i++)
        {
            backgroundSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void LateUpdate()
    {
        cam = Camera.main.transform;
        distance = cam.position.x - camStartPos.x;
        transform.position = new Vector3(cam.position.x, cam.position.y, 0);
        transform.localScale = new Vector3(-cam.position.z / 10, -cam.position.z / 10, 1);



        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backgroundSpeed[i] * parallaxSpeed;
            materials[i].mainTextureOffset = new Vector2(-distance * speed, 0f);
        }
    }
}
