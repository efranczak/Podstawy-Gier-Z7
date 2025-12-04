using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{   

    public Text gameOverText;
    public Text controlsText;
    public Button restartButton;
    public Button startButton;
    public Image background;
    private bool startFlag = true;

    void Start()
    {   

        HideGameOver();

        PauseGame();
        controlsText.enabled = true;
        background.enabled = true;

        restartButton.onClick.AddListener(RestartLevel);
        startButton.onClick.AddListener(StartGame);
        
    }

    public void StartGame()
    {
        ResumeGame();
        controlsText.enabled = false;
        background.enabled = false;
        startButton.gameObject.SetActive(false);
        startFlag = false;
    }

    public void TriggerGameOver()
    {
        PauseGame();
        gameOverText.text = "You Died";
        gameOverText.enabled = true;
        controlsText.enabled = true;
        restartButton.gameObject.SetActive(true);
        background.enabled = true;
    }

    public void TriggerYouWon()
    {   
        PauseGame();
        gameOverText.text = "You Won!";
        gameOverText.enabled = true;
        restartButton.gameObject.SetActive(true);
        background.enabled = true;
    }

    public void HideGameOver()
    {
        gameOverText.enabled = false;
        controlsText.enabled = false;
        background.enabled = false;
        restartButton.gameObject.SetActive(false);
    }

    public void RestartLevel()
    {   
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
