using UnityEngine;

public class rainbow_apple : MonoBehaviour
{
    public float saturation = 0.7f;
    public float value = 1f;

    private SpriteRenderer spriteRenderer;
    private float hue;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (hue >= 1.0f)
        {
            hue = 0.0f;
        }
        else
        {
            hue += 0.01f;
        }

        spriteRenderer.color = Color.HSVToRGB(hue, saturation, value);

    }
}
