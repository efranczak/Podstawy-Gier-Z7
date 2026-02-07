using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip[] appleCollectSounds;

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
        if (appleCollectSounds == null || appleCollectSounds.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, appleCollectSounds.Length);
        AudioClip clip = appleCollectSounds[index];
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, appleVolume);
        }
    }
}
