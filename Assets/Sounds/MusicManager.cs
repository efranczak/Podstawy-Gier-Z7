using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Clips")]
    [SerializeField] private AudioClip gameplayClip;
    [SerializeField] private AudioClip platformClip;

    [Header("Behavior")]
    [SerializeField] private float fadeDuration = 0.9f;
    [SerializeField] private bool persistAcrossScenes = true;

    private AudioSource _gameplaySource;
    private AudioSource _platformSource;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        _gameplaySource = GetComponent<AudioSource>();
        if (_gameplaySource == null)
        {
            _gameplaySource = gameObject.AddComponent<AudioSource>();
        }

        _platformSource = gameObject.AddComponent<AudioSource>();

        _gameplaySource.loop = true;
        _platformSource.loop = true;
    }

    private void Start()
    {
        StartSyncedPlayback();
    }

    public void SwitchToPlatform()
    {
        FadeToMix(0f, 1f);
    }

    public void SwitchToGameplay()
    {
        FadeToMix(1f, 0f);
    }

    private void StartSyncedPlayback()
    {
        if (gameplayClip == null || platformClip == null)
        {
            if (gameplayClip != null)
            {
                _gameplaySource.clip = gameplayClip;
                _gameplaySource.volume = 1f;
                _gameplaySource.Play();
            }
            return;
        }

        _gameplaySource.clip = gameplayClip;
        _platformSource.clip = platformClip;

        _gameplaySource.volume = 1f;
        _platformSource.volume = 0f;

        double startTime = AudioSettings.dspTime + 0.1d;
        _gameplaySource.PlayScheduled(startTime);
        _platformSource.PlayScheduled(startTime);
    }

    private void FadeToMix(float gameplayTarget, float platformTarget)
    {
        if (_platformSource == null || _gameplaySource == null)
        {
            return;
        }

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeMix(gameplayTarget, platformTarget));
    }

    private IEnumerator FadeMix(float gameplayTarget, float platformTarget)
    {
        float startGameplay = _gameplaySource.volume;
        float startPlatform = _platformSource.volume;
        float duration = Mathf.Max(0.01f, fadeDuration);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            _gameplaySource.volume = Mathf.Lerp(startGameplay, gameplayTarget, lerp);
            _platformSource.volume = Mathf.Lerp(startPlatform, platformTarget, lerp);
            yield return null;
        }

        _gameplaySource.volume = gameplayTarget;
        _platformSource.volume = platformTarget;
        _fadeRoutine = null;
    }
}
