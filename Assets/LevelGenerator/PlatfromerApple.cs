using UnityEngine;

public class PlatfromerApple : MonoBehaviour
{
    public float saturation = 0.7f;
    public float value = 1f;
    public bool isCollected = false;

    [Header("Audio")]
    public AudioClip[] collectSounds;
    [Range(0f, 1f)] public float collectVolume = 0.8f;

    private SpriteRenderer spriteRenderer;
    private float hue;
    private BoxCollider2D boxCollider;

    private float velocity;
    private float acceleration;
    private float maxVelocity = 30.0f;

    private bool isMoving = false;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        acceleration = 2.0f;
    }

    void Update()
    {
        if (!isMoving) return;
        acceleration += 0.1f * Time.deltaTime;
        velocity += acceleration;
        if (velocity > maxVelocity)
        {
            velocity = maxVelocity;
        }
        transform.position = new Vector3(transform.position.x + velocity * Time.deltaTime, transform.position.y + 1.5f * Time.deltaTime, transform.position.z);

    }





    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isMoving = true;
        }
        if (collision.CompareTag("tail"))
        {
            isCollected = true;
            PlayCollectSound();
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

    void CollectedEvent()
    {
        // Placeholder for collected event logic
    }

    private void PlayCollectSound()
    {
        if (collectSounds == null || collectSounds.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, collectSounds.Length);
        AudioClip clip = collectSounds[index];
        if (clip != null)
        {
            PlayOneShot2D(clip, collectVolume);
        }
    }

    private void PlayOneShot2D(AudioClip clip, float volume)
    {
        GameObject sfx = new GameObject("SfxOneShot");
        sfx.transform.position = transform.position;

        AudioSource source = sfx.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(sfx, clip.length);
    }
}
