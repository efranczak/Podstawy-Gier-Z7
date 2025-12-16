using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private UpgradeData[] _allUpgrades; 
    [SerializeField] private UpgradeButton[] _upgradeButtons; 
    [SerializeField] private PlayerController _player;

    public SnakeLogic _snakeLogic;

    private int selectedIndex = 0;

    [SerializeField] private int _maxSkillSlots = 2;
    private List<UpgradeType> _unlockedTypes = new List<UpgradeType>();

    #region Input System

    private PlayerInputActions _playerInputActions;
    private InputAction _navigateAction;

    public void RegisterUpgradeChoice(UpgradeType type)
    {
        if (!_unlockedTypes.Contains(type))
        {
            _unlockedTypes.Add(type);
        }
    }

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
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

    public void OnBecameActive()
    {
        selectedIndex = 0;
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

        IEnumerable<UpgradeData> availableUpgrades;

        if (_unlockedTypes.Count >= _maxSkillSlots) {
            availableUpgrades = _allUpgrades.Where(x => _unlockedTypes.Contains(x.type));
        }
        else {
            availableUpgrades = _allUpgrades;
        }

        var chosen = availableUpgrades.OrderBy(x => Random.value).Take(_upgradeButtons.Length).ToArray();

        for (int i = 0; i < _upgradeButtons.Length; i++)
        {
            _upgradeButtons[i].Setup(chosen[i], _player, i == selectedIndex);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);

    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigation = context.ReadValue<Vector2>();
        if (navigation.x > 0)
        {
            selectedIndex = (selectedIndex + 1) % _upgradeButtons.Length;
        }
        else if (navigation.x < 0)
        {
            selectedIndex = (selectedIndex - 1 + _upgradeButtons.Length) % _upgradeButtons.Length;
        }
        for (int i = 0; i < _upgradeButtons.Length; i++)
        {
            _upgradeButtons[i].setIsChosen(i == selectedIndex);
        }
    }
}
