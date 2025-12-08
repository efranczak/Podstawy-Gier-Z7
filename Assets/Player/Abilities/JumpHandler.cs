using UnityEngine;

public class JumpHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] public int _maxJumps = 1;
    [SerializeField] private bool _enabled = true;

    [Header("Handlers")]
    [SerializeField] private WallJumpHandler _wallJumpHandler;

    public int _jumpCount;

    private void Awake()
    {
        if (_player == null) _player = GetComponentInParent<PlayerController>();
        if (_wallJumpHandler == null) _wallJumpHandler = _player.GetComponent<WallJumpHandler>();
    }

    private void OnEnable()
    {
        _player.GroundedChanged += OnGroundedChanged;
    }

    private void OnDisable()
    {
        _player.GroundedChanged -= OnGroundedChanged;
    }

    private void Update()
    {
        if (!_enabled) return;

        if (Input.GetButtonDown("Jump"))
        {
            if (_wallJumpHandler != null && _wallJumpHandler.HasCoyoteTime)
            {
                return;
            }

            if (_jumpCount < _maxJumps)
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
