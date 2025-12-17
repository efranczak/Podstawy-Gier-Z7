using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private UpgradeData[] _allUpgrades; 
    [SerializeField] private UpgradeButton[] _upgradeButtons; 
    [SerializeField] private PlayerController _player;

    public SnakeLogic _snakeLogic;
    private bool _inputLocked = false;

    private int selectedIndex = 0;

    [SerializeField] private int _maxSkillSlots = 2;
    private List<UpgradeType> _unlockedTypes = new List<UpgradeType>();

    #region Input System

    private PlayerInputActions _playerInputActions;
    private InputAction _navigateAction;

    private ActiveUpgradesContainer _activeUpgradesContainer;

    public void RegisterUpgradeChoice(UpgradeData upgrade)
    {
        if (upgrade.category == UpgradeCategory.Permanent && !_unlockedTypes.Contains(upgrade.type))
        {
            _activeUpgradesContainer.UpdateActiveUpgrades(upgrade);
            _unlockedTypes.Add(upgrade.type);
        }
        if (upgrade.category == UpgradeCategory.Consumable)
        {
            _activeUpgradesContainer.UpdateConsumableUpgrades(upgrade);
        }
    }

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _activeUpgradesContainer = FindAnyObjectByType<ActiveUpgradesContainer>();
    }

    private void OnEnable()
    {
        _navigateAction = _playerInputActions.UI.Navigate;
        _navigateAction.Enable();
        _navigateAction.performed += ctx => OnNavigate(ctx);
    }

    private void OnDisable()
    {
        _navigateAction.performed -= ctx => OnNavigate(ctx);
        _navigateAction.Disable();
    }

    #endregion


    void Start()
    {
        Hide();
    }



    public void UpgradeSelected()
    {
        Time.timeScale = 1f;
        _snakeLogic.DecreaseHunger(-_snakeLogic.maxHunger); 
        Hide();
    }

    public void ShowUpgradeSelection()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        selectedIndex = 0;

        IEnumerable<UpgradeData> availableUpgrades;

        if (_unlockedTypes.Count >= _maxSkillSlots)
        {
            availableUpgrades = _allUpgrades.Where(x =>
                _unlockedTypes.Contains(x.type) ||
                x.category == UpgradeCategory.Consumable
            );
        }
        else
        {
            availableUpgrades = _allUpgrades;
        }

        var chosen = availableUpgrades.OrderBy(x => Random.value).Take(_upgradeButtons.Length).ToArray();

        for (int i = 0; i < _upgradeButtons.Length; i++)
        {
            _upgradeButtons[i].Setup(chosen[i], _player, i == selectedIndex);
        }

        StartCoroutine(LockInputTemporarily());


    }
    public void Hide()
    {
        gameObject.SetActive(false);

    }

    public void OnNavigate(InputAction.CallbackContext context)
    {   
        if (_inputLocked) return;

        Vector2 navigation = context.ReadValue<Vector2>();
        if (navigation.x > 0)
        {
            selectedIndex = 1;
        }
        else if (navigation.x < 0)
        {
            selectedIndex = 0;
        }
        for (int i = 0; i < _upgradeButtons.Length; i++)
        {
            _upgradeButtons[i].setIsChosen(i == selectedIndex);
        }
    }

    private IEnumerator LockInputTemporarily()
    {
        _inputLocked = true;
        yield return new WaitForSecondsRealtime(0.5f);
        _inputLocked = false;
    }



}
