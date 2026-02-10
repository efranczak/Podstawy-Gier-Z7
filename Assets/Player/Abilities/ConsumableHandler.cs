using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ConsumableHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private ScriptableStats _baseStats;

    [Header("Sprint Settings")]
    [Tooltip("Czas po którym prêdkoœæ gracza powinna powróciæ do bazowej")]
    [SerializeField] private float _sprintDuration = 15.0f;
    
    [Tooltip("Ilukrotnie zwiêkszyæ bazow¹ prêdkoœæ")]
    [SerializeField] private float _sprintSpeedBoostMultiplier = 1.5f;

    [Header("JumpPower Settings")]
    [Tooltip("Czas po którym moc skoku gracza powinna powróciæ do bazowej")]
    [SerializeField] private float _jumpPowerDuration = 15.0f;

    [Tooltip("Ilukrotnie zwiêkszyæ bazowy skok")]
    [SerializeField] private float _jumpPowerBoostMultiplier = 1.5f;

    [Header("Fall Speed Settings")]
    [Tooltip("Czas po którym predkosc opadania gracza powinna powróciæ do bazowej")]
    [SerializeField] private float _fallSpeedDuration = 20.0f;

    [Tooltip("Ilukrotnie zmniejszyc predkosc opadania")]
    [SerializeField] private float _fallSpeedAmount = 0.1f;

    private UpgradeType? _currentConsumable = null;
    private bool _isEffectActive = false;
    private float _remainingTime = 0f;

    private SpriteRenderer _spriteRenderer;

    private PlayerInputActions _inputActions;
    private InputAction _useAction;

    private ActiveUpgradesContainer _activeUpgradesContainer;


    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _activeUpgradesContainer = FindAnyObjectByType<ActiveUpgradesContainer>();

        if (_activeUpgradesContainer == null)
        {
            Debug.LogError($"[UpgradeButton] Nie znaleziono ActiveUpgradesContainer na scenie {gameObject.name}!");
        }
    }

    private void Start()
    {
        if (_player != null)
        {
            _spriteRenderer = _player.GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        _useAction = _inputActions.Player.Interact;
        _useAction.Enable();
    }

    private void OnDisable()
    {
        _useAction.Disable();
    }

    private void Update()
    {
        if (_useAction.WasPressedThisFrame() && _currentConsumable.HasValue)
        {
            ToggleConsumable();
        }

        if (_isEffectActive && _remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime <= 0)
            {
                FinishConsumable();
            }
        }
    }

    public void SetConsumable(UpgradeType type)
    {
        if (_currentConsumable.HasValue)
        {
            DeactivateEffect();
        }

        _currentConsumable = type;
        _isEffectActive = false;

        switch (type)
        {
            case UpgradeType.Sprint: _remainingTime = _sprintDuration; break;
            case UpgradeType.JumpPower: _remainingTime = _jumpPowerDuration; break;
            case UpgradeType.FallSpeed: _remainingTime = _fallSpeedDuration; break;
        }

        Debug.Log($"Picked up consumable: {type}. Time: {_remainingTime}s");
    }

    private void ToggleConsumable()
    {
        if (_isEffectActive)
        {
            DeactivateEffect();

            // Kara za pauzowanie: 1s
            _remainingTime -= 1.0f;

            if (_remainingTime <= 0) FinishConsumable();
        }
        else if (_remainingTime > 0) ActivateEffect();
    }
    private void ActivateEffect()
    {
        _isEffectActive = true;
        ApplyStats(true);

        if (_spriteRenderer != null && _currentConsumable.HasValue)
        {
            switch (_currentConsumable.Value)
            {
                case UpgradeType.Sprint:
                    _spriteRenderer.color = Color.blue;
                    break;
                case UpgradeType.JumpPower:
                    _spriteRenderer.color = Color.red;
                    break;
                case UpgradeType.FallSpeed:
                    _spriteRenderer.color = Color.green;
                    break;
            }
        }
    }
    private void DeactivateEffect()
    {
        _isEffectActive = false;
        ApplyStats(false);

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white;
        }
    }
    private void FinishConsumable()
    {
        Debug.Log("Consumable finished!");
        DeactivateEffect();
        _currentConsumable = null;
        _remainingTime = 0;
        _activeUpgradesContainer.UpdateConsumableUpgrades(null);
    }
    private void ApplyStats(bool applyBoost)
    {
        ScriptableStats currentStats = _player.Stats;
        if (currentStats == null || !_currentConsumable.HasValue) return;

        switch (_currentConsumable)
        {
            case UpgradeType.Sprint:
                currentStats.MaxSpeed = _baseStats.MaxSpeed;
                currentStats.Acceleration = _baseStats.Acceleration;
                break;
            case UpgradeType.JumpPower:
                currentStats.JumpPower = _baseStats.JumpPower;
                currentStats.MaxFallSpeed = _baseStats.MaxFallSpeed;
                break;
            case UpgradeType.FallSpeed:
                currentStats.MaxFallSpeed = _baseStats.MaxFallSpeed;
                break;
        }

        if (applyBoost)
        {
            switch (_currentConsumable)
            {
                case UpgradeType.Sprint:
                    currentStats.MaxSpeed *= _sprintSpeedBoostMultiplier;
                    currentStats.Acceleration *= _sprintSpeedBoostMultiplier;
                    break;
                case UpgradeType.JumpPower:
                    currentStats.JumpPower *= _jumpPowerBoostMultiplier;
                    currentStats.MaxFallSpeed *= _jumpPowerBoostMultiplier;
                    break;
                case UpgradeType.FallSpeed:
                    currentStats.MaxFallSpeed *= _fallSpeedAmount;
                    break;
            }
        }
    }
    public float GetTimeRemaining()
    {
        return _currentConsumable.HasValue ? Mathf.Max(0, _remainingTime) : 0f;
    }

    public bool IsActive()
    {
        return _isEffectActive;
    }
}