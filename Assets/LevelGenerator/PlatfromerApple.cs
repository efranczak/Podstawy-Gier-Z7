using UnityEngine;

public class PlatfromerApple : MonoBehaviour
{
    public float saturation = 0.7f;
    public float value = 1f;
    public bool isCollected = false;

    private SpriteRenderer spriteRenderer;
    private float hue;
    private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            gameObject.SetActive(false);
        }
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
