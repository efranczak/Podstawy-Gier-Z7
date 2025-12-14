using UnityEngine;
using UnityEngine.InputSystem;

public class JumpHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private bool _enabled = true;
    [SerializeField] private PlayerSkills _playerSkills;

    [Header("Handlers")]
    [SerializeField] private WallJumpHandler _wallJumpHandler;

    public int _jumpCount;

    private PlayerInputActions _inputActions;
    private InputAction jumpAction;

    private void Awake()
    {
        if (_player == null) _player = GetComponentInParent<PlayerController>();
        if (_wallJumpHandler == null) _wallJumpHandler = _player.GetComponent<WallJumpHandler>();
        _playerSkills = _player.GetComponent<PlayerSkills>();
        if (_playerSkills == null)
        {
            enabled = false;
        }

        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _player.GroundedChanged += OnGroundedChanged;

        jumpAction = _inputActions.Player.Jump;
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        _player.GroundedChanged -= OnGroundedChanged;

        jumpAction.Disable();
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }

    private void Update()
    {
        if (!_enabled) return;

        if (jumpAction.WasPressedThisFrame())
        {
            if (_wallJumpHandler != null && _wallJumpHandler.HasCoyoteTime)
            {
                return;
            }

            if (_jumpCount < _playerSkills.PlayerJumps)
            {
                _jumpCount++;
                ForceJump();
            }
        }
    }

    private void OnGroundedChanged(bool grounded, float impactVelocity)
    {
        if (grounded)
        {
            _jumpCount = 0;
        }
    }

    private void ForceJump()
    {
        var stats = typeof(PlayerController)
            .GetField("_stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_player) as ScriptableStats;

        _player.ForceJump(stats.JumpPower * stats.DoubleJumpPower);

        if (stats != null)
        {
            _player.ForceJump(stats.JumpPower);
        }
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }
}
