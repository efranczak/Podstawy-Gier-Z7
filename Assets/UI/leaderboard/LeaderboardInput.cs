using UnityEngine;
using UnityEngine.InputSystem;

public class LeaderboardInput : MonoBehaviour
{
    private bool isSelected = false;
    private int currentLetter = 0;
    private char[] playerName = "AAA".ToCharArray();

    private bool _inputLocked = false;
    private GameOverScript _gameOverScript;

    private PlayerInputActions _playerInputActions;
    private InputAction _navigateAction;
    private InputAction _selectAction;
    private InputAction _cancelAction;

    public LeaderboardInputLetter[] leaderboardInputLetters;
    public LeaderboardManager leaderboardManager;

    #region Input System

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _gameOverScript = FindAnyObjectByType<GameOverScript>();
        leaderboardManager = FindAnyObjectByType<LeaderboardManager>();
        // _activeUpgradesContainer = FindAnyObjectByType<ActiveUpgradesContainer>();
    }

    private void OnEnable()
    {
        _navigateAction = _playerInputActions.UI.Navigate;
        _navigateAction.performed += OnNavigate;
        _navigateAction.Enable();

        _selectAction = _playerInputActions.UI.Select;
        _selectAction.performed += OnSelect;
        _selectAction.Enable();

        _cancelAction = _playerInputActions.UI.Cancel1;
        _cancelAction.performed += OnCancel;
        _cancelAction.Enable();
    }

    private void OnDisable()
    {
        _navigateAction.performed -= OnNavigate;
        _navigateAction.Disable();

        _selectAction.performed -= OnSelect;
        _selectAction.Disable();

        _cancelAction.performed -= OnCancel;
        _cancelAction.Disable();
    }


    #endregion

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (_inputLocked) return;

        Vector2 navigation = context.ReadValue<Vector2>();
        if (navigation.y > 0.3f)
        {
            leaderboardInputLetters[currentLetter].IncrementLetter();
        }
        else if (navigation.y < -0.3f)
        {
            leaderboardInputLetters[currentLetter].DecrementLetter();
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (_inputLocked) return;
        leaderboardInputLetters[currentLetter].SetNotActive();
        currentLetter++;
        if (currentLetter >= leaderboardInputLetters.Length)
        {
            playerName[currentLetter - 1] = leaderboardInputLetters[currentLetter - 1].GetCurrentLetter();
            string finalName = new string(playerName);
            leaderboardManager.AddScore(finalName, ScoreManager.instance != null ? ScoreManager.instance.GetScore() : 0);

            _gameOverScript.RestartLevel();
            // END OF LEADERBOARD INPUT HERE
        }
        else
        {
            playerName[currentLetter-1] = leaderboardInputLetters[currentLetter-1].GetCurrentLetter();
            leaderboardInputLetters[currentLetter].SetActive();
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (_inputLocked) return;
        leaderboardInputLetters[currentLetter].SetNotActive();
        currentLetter--;
        if (currentLetter < 0)
        {
            currentLetter = 0;
        }

        leaderboardInputLetters[currentLetter].SetActive();
    }


    void Start()
    {
        leaderboardInputLetters[currentLetter].SetActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
