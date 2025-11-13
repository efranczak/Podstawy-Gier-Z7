using UnityEngine;

public class DoubleJumpHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private int _maxJumps = 2;
    [SerializeField] private bool _enabled = false;

    public int _jumpCount;

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;
    }

    private void Update()
    {
        if (!_enabled) return;

        if (Input.GetButtonDown("Jump") && _jumpCount < _maxJumps)
        {
            if (_jumpCount >= 1) {
                ForceJump();
            }
        }
    }

    private void OnJumped()
    {
        _jumpCount++;
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
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }
}
