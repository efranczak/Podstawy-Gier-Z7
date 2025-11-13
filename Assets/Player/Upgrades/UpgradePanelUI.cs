using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] private UpgradeData[] _allUpgrades; 
    [SerializeField] private UpgradeButton[] _upgradeButtons; 
    [SerializeField] private PlayerController _player;

    public SnakeLogic _snakeLogic;


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

        var chosen = _allUpgrades.OrderBy(x => Random.value).Take(_upgradeButtons.Length).ToArray();

        for (int i = 0; i < _upgradeButtons.Length; i++)
        {
            _upgradeButtons[i].Setup(chosen[i], _player);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);

    }
}
