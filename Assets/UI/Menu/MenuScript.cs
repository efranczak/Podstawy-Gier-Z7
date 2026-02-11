using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using DG.Tweening;

public class MenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject firstMenu;
    public TMP_Text promptText;
    public GameObject title;

    public float blinkInterval = 0.5f;

    #region Input System

    private PlayerInputActions playerInputActions;
    private InputAction anyButtonUI;
    private bool lockAnyButton = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        anyButtonUI = playerInputActions.UI.AnyButton;
    }

    private void OnEnable()
    {
        anyButtonUI.Enable();
        anyButtonUI.performed += ctx =>
        {
            if (!lockAnyButton)
            {
                lockAnyButton = true;
                firstMenu.SetActive(false);
                RectTransform rect = title.GetComponent<RectTransform>();

                rect.DOAnchorPosY(
                    rect.anchoredPosition.y + 90f,
                    2f
                ).SetEase(Ease.OutCubic)
                 .OnComplete(() =>
                 {
                     mainMenu.SetActive(true);
                     mainMenu.GetComponent<MainMenuScript>().ShowMenu();
                 });


            }
        };
    }

    private void OnDisable()
    {
        anyButtonUI.Disable();
    }

    #endregion


    void Start()
    {
        firstMenu.SetActive(true);
        mainMenu.SetActive(false);
        StartCoroutine(BlinkingCoroutine());
    }

    IEnumerator BlinkingCoroutine()
    {
        while (true && !lockAnyButton)
        {
            promptText.enabled = !promptText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
        promptText.enabled = true;
        yield return new WaitForSeconds(blinkInterval);
        promptText.enabled = false;
    }
}
