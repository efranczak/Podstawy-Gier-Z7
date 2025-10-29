using UnityEngine;

public class parallax : MonoBehaviour
{
    Material material;
    float distance;

    [Range(0f, 0.5f)]
    public float speed;


    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        distance += speed * Time.deltaTime;
        material.mainTextureOffset = new Vector2(distance, 0f);
    }
}
