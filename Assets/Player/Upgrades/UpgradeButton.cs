using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Outline outline;

    private UpgradeData _upgradeData;
    private PlayerController _player;
    private bool isChosen = false;
    //public SnakeScript _snake;

    private PlayerInputActions _playerInputActions;
    private InputAction _selectAction;

    private UpgradePanelUI _upgradePanel;
    private bool wasClickedOnce = false;
    private bool wasSelected = false;

    private ActiveUpgradesContainer _activeUpgradesContainer;

    private void Awake()
    {
        _upgradePanel = GetComponentInParent<UpgradePanelUI>();
        _activeUpgradesContainer = FindAnyObjectByType<ActiveUpgradesContainer>();

        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _selectAction = _playerInputActions.UI.Select;
        _selectAction.Enable();
        _selectAction.performed += ctx => OnSelected();
    }

    private void OnDisable()
    {
        _selectAction.performed -= ctx => OnSelected();
        _selectAction.Disable();
    }

    private void OnDestroy()
    {
        _playerInputActions.Dispose();
    }

    public void setIsChosen(bool chosen)
    {
        isChosen = chosen;
    }

    public void Setup(UpgradeData data, PlayerController player, bool isChosen)
    {
        _upgradeData = data;
        _player = player;

        _icon = data.icon;
        _title.text = data.upgradeName;
        _description.text = data.description;

        this.isChosen = isChosen;

        wasClickedOnce = false;
        wasSelected = false;

    }

    private void Update()
    {
        if (isChosen)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }

    private void OnSelected()
    {   
        if (!isChosen) return;

        //second click
        if (wasClickedOnce)
        {
            _activeUpgradesContainer.UpdateActiveUpgrades(_upgradeData);
            _upgradePanel.RegisterUpgradeChoice(_upgradeData.type);
            _upgradeData.ApplyUpgrade(_player/*, _snake*/);
            _upgradePanel.UpgradeSelected();
            wasSelected = true;
            return;
        }


        // first click
        wasClickedOnce = true;
        StartCoroutine(WaitForConfirmation());
    }

    private IEnumerator WaitForConfirmation()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (!wasSelected && isChosen)
        {   
            _activeUpgradesContainer.UpdateActiveUpgrades(_upgradeData);
            _upgradePanel.RegisterUpgradeChoice(_upgradeData.type);
            _upgradeData.ApplyUpgrade(_player/*, _snake*/);
            _upgradePanel.UpgradeSelected();
            wasSelected = true;
        }
    }
}

