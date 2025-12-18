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
    [SerializeField] private float _sprintDuration = 9.0f;
    
    [Tooltip("Ilukrotnie zwiêkszyæ bazow¹ prêdkoœæ")]
    [SerializeField] private float _sprintSpeedBoostMultiplier = 1.5f;

    [Header("JumpPower Settings")]
    [Tooltip("Czas po którym moc skoku gracza powinna powróciæ do bazowej")]
    [SerializeField] private float _jumpPowerDuration = 9.0f;

    [Tooltip("Ilukrotnie zwiêkszyæ bazowy skok")]
    [SerializeField] private float _jumpPowerBoostMultiplier = 1.5f;

    [Header("Fall Speed Settings")]
    [Tooltip("Czas po którym predkosc opadania gracza powinna powróciæ do bazowej")]
    [SerializeField] private float _fallSpeedDuration = 15.0f;

    [Tooltip("Ilukrotnie zmniejszyc predkosc opadania")]
    [SerializeField] private float _fallSpeedAmount = 0.1f;

    private UpgradeType? _currentConsumable = null;
    private Coroutine _activeEffectCoroutine;

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
        if (_useAction.WasPressedThisFrame())
        {
            Debug.Log("Use action pressed");
        }

        if (_useAction.WasPressedThisFrame() && _currentConsumable.HasValue)
        {
            ActivateConsumable();
        }
    }

    public void SetConsumable(UpgradeType type)
    {
        _currentConsumable = type;
        Debug.Log($"Picked up consumable: {type}");
    }

    private void ActivateConsumable()
    {
        Debug.Log("ActivateConsumable called");

        if (_activeEffectCoroutine != null) StopCoroutine(_activeEffectCoroutine);

        switch (_currentConsumable)
        {
            case UpgradeType.Sprint:
                _activeEffectCoroutine = StartCoroutine(SprintRoutine(_sprintDuration, _sprintSpeedBoostMultiplier));
                GameObject.FindWithTag("Player").GetComponentInChildren<SpriteShading>().Flash(Color.blue, _sprintDuration);
                break;
            case UpgradeType.JumpPower:
                _activeEffectCoroutine = StartCoroutine(JumpPowerRoutine(_jumpPowerDuration, _jumpPowerBoostMultiplier));
                GameObject.FindWithTag("Player").GetComponentInChildren<SpriteShading>().Flash(Color.red, _jumpPowerDuration);
                break;
            case UpgradeType.FallSpeed:
                _activeEffectCoroutine = StartCoroutine(FallSpeedRoutine(_fallSpeedDuration, _fallSpeedAmount));
                GameObject.FindWithTag("Player").GetComponentInChildren<SpriteShading>().Flash(Color.green, _jumpPowerDuration);
                break;
        }

        _currentConsumable = null;
        _activeUpgradesContainer.UpdateConsumableUpgrades(null);
    }

    private IEnumerator SprintRoutine(float duration, float multiplier)
    {
        Debug.Log("SprintRoutine called");

        ScriptableStats currentStats = _player.Stats;

        if (currentStats == null) yield break;

        float originalMaxSpeed = _baseStats.MaxSpeed;
        float originalAcceleration = _baseStats.Acceleration;

        currentStats.MaxSpeed *= multiplier;
        currentStats.Acceleration *= multiplier;

        yield return new WaitForSeconds(duration);

        currentStats.MaxSpeed = originalMaxSpeed;
        currentStats.Acceleration = originalAcceleration;

        _activeEffectCoroutine = null;
    }

    private IEnumerator JumpPowerRoutine(float duration, float multiplier)
    {
        Debug.Log("SprintRoutine called");

        ScriptableStats currentStats = _player.Stats;

        if (currentStats == null) yield break;

        float originalJumpPower = _baseStats.JumpPower;
        float originalFallSpeed = _baseStats.MaxFallSpeed;

        currentStats.JumpPower *= multiplier;
        currentStats.MaxFallSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        currentStats.JumpPower = originalJumpPower;
        currentStats.MaxFallSpeed = originalFallSpeed;

        _activeEffectCoroutine = null;
    }

    private IEnumerator FallSpeedRoutine(float duration, float multiplier)
    {
        Debug.Log("SprintRoutine called");

        ScriptableStats currentStats = _player.Stats;

        if (currentStats == null) yield break;

        float originalFallSpeed = _baseStats.MaxFallSpeed;

        currentStats.MaxFallSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        currentStats.MaxFallSpeed = originalFallSpeed;

        _activeEffectCoroutine = null;
    }
}