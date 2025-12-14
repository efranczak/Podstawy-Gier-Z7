using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Xml.Serialization;

public class DashHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private bool _enabled = true;

    [Header("Dash Settings")]
    [Tooltip("Jak daleko gracz dashuje")]
    public float DashDistance = 6f;

    [Tooltip("Czas trwania dasha w sekundach")]
    public float DashDuration = 0.2f;

    [Tooltip("Czas oczekiwania po dashu, zanim można wykonać kolejny w serii kilku dashy")]
    public float DashCooldown = 0.1f;

    [SerializeField] private PlayerSkills _playerSkills;

    private bool _canDash = true;
    private int _currentConsecutiveDashes = 0;

    private PlayerInputActions _inputActions;
    private InputAction dashAction;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

        if (_playerSkills == null) _playerSkills = _player.GetComponent<PlayerSkills>();

        if (_playerSkills == null)
        {
            enabled = false;
        }
    }
    private void OnEnable()
    {
        _player.GroundedChanged += OnGroundedChanged;

        dashAction = _inputActions.Player.Dash;
        dashAction.Enable();

    }

    private void OnDisable()
    {
        _player.GroundedChanged -= OnGroundedChanged;
        dashAction.Disable();
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }


    private void Update()
    {
        if (!_enabled || !_canDash || _playerSkills == null) return;


        if (_player.IsClimbing && _currentConsecutiveDashes > 0)
        {
            ResetDashChain();
        }

        if (dashAction.WasPressedThisFrame())
        {
            TryDash();
        }
    }

    private void OnGroundedChanged(bool grounded, float impactVelocity)
    {
        if (grounded)
        {
            ResetDashChain();
        }
    }

    public void ResetDashChain()
    {
        _currentConsecutiveDashes = 0;
        _canDash = true;
    }

    private void TryDash()
    {
        if (_currentConsecutiveDashes < _playerSkills.PlayerDashes)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        _canDash = false;
        _currentConsecutiveDashes++;

        float direction = _player.GetFacingDirection();

        Vector2 dashVelocity = new Vector2(direction * (DashDistance / DashDuration), 0f);

        _player.AddExternalVelocity(dashVelocity, DashDuration);

        yield return new WaitForSeconds(DashDuration);

        if (_currentConsecutiveDashes < _playerSkills.PlayerDashes)
        {
            yield return new WaitForSeconds(DashCooldown);

            _canDash = true;
        }
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }
}
