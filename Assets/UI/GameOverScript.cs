using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{   

    public Text gameOverText;
    public Button restartButton;

    void Start()
    {   
        HideGameOver();
        restartButton.onClick.AddListener(RestartLevel);
    }

    public void TriggerGameOver()
    {
        PauseGame();
        gameOverText.text = "You Died";
        gameOverText.enabled = true;
        restartButton.gameObject.SetActive(true);
    }

    public void TriggerYouWon()
    {   
        PauseGame();
        gameOverText.text = "You Won!";
        gameOverText.enabled = true;
        restartButton.gameObject.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverText.enabled = false;
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
