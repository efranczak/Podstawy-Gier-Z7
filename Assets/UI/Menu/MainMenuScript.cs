using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private int selectedIndex = 0;
    private float[] buttonYPositions = new float[] { -35f, -70f, -105f, -141f };

    public GameObject ButtonBackground;
    public CanvasGroup canvasGroup;
    public TMP_Text[] menuOptions;
    public GameObject title;
    public CanvasGroup creditsPanel;

    private bool isInCredits = false;
    private bool isTransitioning = false;



    #region Input System

    private PlayerInputActions playerInputActions;
    private InputAction horizontal;
    private InputAction select;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        horizontal = playerInputActions.UI.Navigate;
        select = playerInputActions.UI.Select;
    }

    private void OnEnable()
    {
        horizontal.Enable();
        horizontal.performed += ctx => navigateMenu();

        select.Enable();
        select.performed += ctx => onSelect();
    }

    private void OnDisable()
    {
        horizontal.Disable();
        horizontal.performed -= ctx => navigateMenu();

        select.Disable();
        select.performed -= ctx => onSelect();
    }

    #endregion




    private void navigateMenu()
    {
        if (isTransitioning || isInCredits)
            return;

        Vector2 navigation = horizontal.ReadValue<Vector2>();
        if (navigation.y > 0.5f)
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
        }
        else if (navigation.y < -0.5f)
        {
            selectedIndex = Mathf.Min(3, selectedIndex + 1);
        }

        ButtonBackground.transform.DOLocalMoveY(buttonYPositions[selectedIndex], 0.2f);
        UpdateMenuVisuals();
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        creditsPanel.alpha = 0f;
    }

    public void ShowMenu()
    {
        canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
    }

    public void ShowCredits()
    {
        if (isTransitioning)
            return;

        isTransitioning = true;


        RectTransform rect = title.GetComponent<RectTransform>();
        canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InCubic);

        rect.DOAnchorPosY(
            rect.anchoredPosition.y + 180f,
            2f
        ).SetEase(Ease.InOutSine)
         .OnComplete(() =>
         {
             creditsPanel.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
             isTransitioning = false;
         });
    }

    public void HideCredits()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        RectTransform rect = title.GetComponent<RectTransform>();
        creditsPanel.DOFade(0f, 0.5f).SetEase(Ease.InCubic);
        rect.DOAnchorPosY(
            rect.anchoredPosition.y - 180f,
            2f
        ).SetEase(Ease.InOutSine)
         .OnComplete(() =>
         {
             ShowMenu();
             isTransitioning = false;
         });
    }

    private void UpdateMenuVisuals()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            RectTransform rect = menuOptions[i].rectTransform;

            rect.DOKill();

            if (i == selectedIndex)
            {
                rect.DOScale(1.1f, 0.15f)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                rect.DOScale(1f, 0.15f)
                    .SetEase(Ease.OutQuad);
            }
        }
    }

    private void onSelect()
    {
        if (isTransitioning)
            return;

        if (isInCredits)
        {
            HideCredits();
            isInCredits = false;
            return;
        }


        switch (selectedIndex)
        {
            case 0:
                // Start Game
                SceneManager.LoadScene(0);
                break;
            case 1:
                // Show Leaderboard
                break;
            case 2:
                ShowCredits();
                isInCredits = true;
                break;
            case 3:
                Application.Quit();
                break;
            default: break;
        }
    }
}




