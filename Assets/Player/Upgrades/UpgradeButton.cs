using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Button _button;

    private UpgradeData _upgradeData;
    private PlayerController _player;

    private UpgradePanelUI _upgradePanel;

    private void Awake()
    {
        _upgradePanel = GetComponentInParent<UpgradePanelUI>();
    }

    public void Setup(UpgradeData data, PlayerController player)
    {
        _upgradeData = data;
        _player = player;

        _icon.sprite = data.icon;
        _title.text = data.upgradeName;
        // _description.text = data.description;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnSelected);
    }

    private void OnSelected()
    {
        _upgradeData.ApplyUpgrade(_player);
        _upgradePanel.UpgradeSelected();
    }
}

