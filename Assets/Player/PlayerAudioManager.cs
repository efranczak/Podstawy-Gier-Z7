using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip appleCollectSound;

    [Header("Settings")]

    [Range(0f, 1f)] public float appleVolume = 0.8f;

    private AudioSource audioSource;
    private bool isRushPlaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayAppleSound()
    {
        if (appleCollectSound != null)
        {
            audioSource.PlayOneShot(appleCollectSound, appleVolume);
        }
    }
}
