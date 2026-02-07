using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public Text scoreText;
    public Text highScoreText;

    [Header("Audio")]
    public AudioClip[] pointCollectSounds;
    [Range(0f, 1f)] public float pointVolume = 1.0f;

    private AudioSource _audioSource;

    int score = 0;
    int highScore = 0;

    private void Awake()
    {
        instance = this;
        highScore = PlayerPrefs.GetInt("highscore", 0);
        Debug.Log($"ScoreManager Awake - loaded highscore = {highScore}");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    void Start()
    {
        scoreText.text = "SCORE: " + score.ToString();
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    public void AddPoint()
    {
        score += 100;
        scoreText.text = "SCORE: " + score.ToString();
        if (highScore < score)
        {
            highScore = score;
            PlayerPrefs.SetInt("highscore", highScore);
            PlayerPrefs.Save();
            highScoreText.text = "HIGH SCORE: " + highScore.ToString();
            Debug.Log($"New highscore = {highScore} saved");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("point"))
        {
            PlayPointSound();
            AddPoint();
            Destroy(other.gameObject);
        }
    }

    private void PlayPointSound()
    {
        if (pointCollectSounds == null || pointCollectSounds.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, pointCollectSounds.Length);
        AudioClip clip = pointCollectSounds[index];
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip, pointVolume);
        }
    }

    public int GetScore()
    {
        return score;
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("highscore", highScore);
        PlayerPrefs.Save();
        Debug.Log($"OnApplicationQuit - saved highscore = {highScore}");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetInt("highscore", highScore);
            PlayerPrefs.Save();
            Debug.Log($"OnApplicationPause - saved highscore = {highScore}");
        }
    }
}
