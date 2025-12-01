using UnityEngine;

public class JumpHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] public int _maxJumps = 1;
    [SerializeField] private bool _enabled = true;

    public int _jumpCount;

    private void OnEnable()
    {
        // USUÑ: _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
    }

    private void OnDisable()
    {
        // USUÑ: _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;
    }

    private void Update()
    {
        if (!_enabled) return;

        if (Input.GetButtonDown("Jump") && _jumpCount < _maxJumps)
        {
            // 1. Zwiêksz licznik
            _jumpCount++; // <-- Zliczamy skok TUTAJ

            // 2. Wykonaj skok
            ForceJump();
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
