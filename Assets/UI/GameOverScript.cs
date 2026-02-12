using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public TMP_Text gameOverText;
    public Image controlsText;
    public Button restartButton;
    public Button startButton;
    public Image background;
    public PauseScript pauseScript;

    [Header("UI elements")]
    public GameObject upgradeSlots;
    public GameObject apples;


    [Header("Leaderboard Settings")]
    public LeaderboardManager leaderboardManager;
    public TMP_InputField nameInputField;
    public GameObject leaderboardPanel;
    public TMP_Text[] nameTexts;
    public TMP_Text[] scoreTexts;
    public TMP_Text currentScore;
    public Button submitButton;

    private PlayerInputActions playerInputActions;
    private InputAction button;
    private InputAction pauseButton;
    private bool startFlag = true;
    private bool pauseFlag = false;
    private bool inPauseMenu = false;

    // guard ¿eby nie dublowaæ submitu
    private bool submitted = false;

    public LeaderboardScript leaderboard_;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }


    private void OnEnable()
    {
        button = playerInputActions.UI.Select;
        button.Enable();

        button.performed += ctx =>
        {
            if (startFlag)
            {
                StartGame();
            }
        };

        pauseButton = playerInputActions.UI.Cancel1;
        pauseButton.Enable();

        pauseButton.performed += ctx =>
        {
            if (!pauseFlag && !inPauseMenu)
            {
                // show pause menu
            }
            else if (pauseFlag && inPauseMenu)
            {
                // hide pause menu
            }
            // else do nothing
        };
    }

    private void OnDisable()
    {
        if (button != null) button.Disable();
    }

    void Start()
    {
        // Upewnij siê, ¿e wszystkie elementy leaderboardu s¹ ukryte na starcie
        HideGameOver();
        PauseGame();

        if (controlsText != null) controlsText.enabled = true;
        if (background != null) background.enabled = true;
        if (startButton != null) startButton.gameObject.SetActive(true);
        if (upgradeSlots != null) upgradeSlots.SetActive(false);
        if (apples != null) upgradeSlots.SetActive(false);

        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        if (startButton != null) startButton.onClick.AddListener(StartGame);

        if (nameTexts != null)
        {
            foreach (var t in nameTexts) if (t != null) t.gameObject.SetActive(false);
        }
        if (scoreTexts != null)
        {
            foreach (var t in scoreTexts) if (t != null) t.gameObject.SetActive(false);
        }

    }

    public void StartGame()
    {
        ResumeGame();
        HideGameOver();
        if (startButton != null) startButton.gameObject.SetActive(false);
        if (upgradeSlots != null) upgradeSlots.SetActive(true);
        if (apples != null) upgradeSlots.SetActive(true);
        startFlag = false;
    }

    public void SubmitScore()
    {
        Debug.Log("[GameOverScript] SubmitScore called");
        if (submitted)
        {
            Debug.Log("[GameOverScript] SubmitScore ignored - already submitted");
            return;
        }

        if (leaderboardManager == null || nameInputField == null)
        {
            Debug.LogWarning("[GameOverScript] leaderboardManager or nameInputField is null");
            return;
        }

        submitted = true;

        nameInputField.gameObject.SetActive(false);
        if (submitButton != null)
        {
            submitButton.gameObject.SetActive(false);
            submitButton.interactable = false;
        }
        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
    }


    public void TriggerGameOver()
    {
        PauseGame();
        submitted = false; // reset flag przy pokazaniu ekranu
        if (upgradeSlots != null) upgradeSlots.SetActive(false);
        if (apples != null) upgradeSlots.SetActive(false);

        if (leaderboard_ != null) leaderboard_.ActivateLeaderboard();

        if (restartButton != null) restartButton.gameObject.SetActive(true);
        if (background != null) background.enabled = true;

        if (nameInputField != null)
        {
            nameInputField.gameObject.SetActive(true);
            nameInputField.text = "";
            // ustaw fokus ¿eby mo¿na by³o wpisywaæ od razu
            EventSystem.current?.SetSelectedGameObject(nameInputField.gameObject);
            nameInputField.ActivateInputField();
        }
        if (submitButton != null)
        {
            submitButton.gameObject.SetActive(true);
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(SubmitScore);
            submitButton.interactable = true;
        }
    }

    public void TriggerYouWon()
    {
        PauseGame();
        if (gameOverText != null) { gameOverText.text = "You Won!"; gameOverText.enabled = true; }

        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
        // UpdateLeaderboardUI();

        if (restartButton != null) restartButton.gameObject.SetActive(true);
        if (background != null) background.enabled = true;
    }

    public void HideGameOver()
    {
        if (gameOverText != null) gameOverText.enabled = false;
        if (controlsText != null) controlsText.enabled = false;
        if (background != null) background.enabled = false;
        if (restartButton != null) restartButton.gameObject.SetActive(false);

        if (nameInputField != null) nameInputField.gameObject.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if (currentScore != null) currentScore.gameObject.SetActive(false);
        if (submitButton != null) submitButton.gameObject.SetActive(false);

        if (nameTexts != null)
        {
            foreach (var t in nameTexts) if (t != null) t.gameObject.SetActive(false);
        }
        if (scoreTexts != null)
        {
            foreach (var t in scoreTexts) if (t != null) t.gameObject.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseFlag = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseFlag = false;
    }

    public void ShowPauseMenu()
    {
        
        inPauseMenu = true;
        PauseGame();
        pauseScript.ShowPause();
    }

    public void HidePauseMenu()
    {
        inPauseMenu = false;
        pauseScript.HidePause();
        ResumeGame();
    }
}