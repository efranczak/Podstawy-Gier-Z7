using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PauseScript : MonoBehaviour
{
    private int currentButton = 0;
    private bool isResumeActive = true; // jesli true to przycisk 0 jest resume, jesli false to new game

    public TMP_Text resumeText;
    public TMP_Text mainMenuText;

    public Animator backgroundAnimator;
    public GameOverScript gameOverScript;

    #region Input System

    private PlayerInputActions playerInputActions;
    private InputAction pauseButton;
    private InputAction navigate;
    private InputAction select;

    private bool isHidden = true;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        navigate = playerInputActions.UI.Navigate;
        select = playerInputActions.UI.Select;
        pauseButton = playerInputActions.UI.Pause;
        navigate.Enable();
        select.Enable();
        pauseButton.Enable();
        navigate.performed += ctx => Navigate(ctx.ReadValue<Vector2>());
        select.performed += ctx => Select();
        pauseButton.performed += ctx =>
        {
            if (isHidden) ShowPause();
            else
            {
                HidePause();
                Time.timeScale = 1f;
            }
        };
    }

    private void OnDisable()
    {
        navigate.Disable();
        select.Disable();

    }

    #endregion

    void Start()
    {
        HidePause();

        RectTransform resumeRect = resumeText.GetComponent<RectTransform>();
        resumeRect.DOScale(1.1f, 0.2f).SetEase(Ease.OutCubic);
        resumeText.color = Color.yellow;
        backgroundAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;


    }


    public void HidePause()
    {
        backgroundAnimator.Play("Idle");
        resumeText.enabled = false;
        mainMenuText.enabled = false;

        isHidden = true;
    }

    public void ShowPause(bool isEnd = false)
    {
        Time.timeScale = 0f;

        if (!isEnd)
        {
            backgroundAnimator.SetTrigger("showAnim");
            isResumeActive = true;
            resumeText.text = "Resume";
            DOVirtual.DelayedCall(1.5f, () =>
            {
                resumeText.enabled = true;
                mainMenuText.enabled = true;
            }).SetUpdate(true);
        }
        else
        {
            backgroundAnimator.SetTrigger("showEnd");
            isResumeActive = false;
            resumeText.text = "New Game";
            DOVirtual.DelayedCall(0.25f, () =>
            {
                resumeText.enabled = true;
                mainMenuText.enabled = true;
            }).SetUpdate(true);
        }

        isHidden = false;


    }


    private void Navigate(Vector2 direction)
    {
        if (isHidden) return;

        if (direction.y > 0.5f || direction.y < -0.5f)
        {
            currentButton = (currentButton + 1) % 2;
        }

        RectTransform resumeRect = resumeText.GetComponent<RectTransform>();
        RectTransform mainMenuRect = mainMenuText.GetComponent<RectTransform>();

        if (currentButton == 0)
        {
            resumeRect.DOScale(1.1f, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true);
            mainMenuRect.DOScale(1f, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true);
            resumeText.color = Color.yellow;
            mainMenuText.color = Color.white;
        }
        else
        {
            resumeRect.DOScale(1f, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true);
            mainMenuRect.DOScale(1.1f, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true);
            resumeText.color = Color.white;
            mainMenuText.color = Color.yellow;
        }

    }

    private void Select()
    { 
        if (isHidden) return;

        switch (currentButton)
        {
            case 0:
                if (isResumeActive)
                {
                    HidePause();
                    Time.timeScale = 1f;
                }
                else gameOverScript.RestartLevel();
                break;
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                break;
        }

    }
    
}
