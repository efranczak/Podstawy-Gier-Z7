using UnityEngine;
using System.Collections;

public class DashHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private bool _enabled = true;

    [Header("Dash Settings")]
    [Tooltip("Jak daleko gracz dashuje w jednostkach Unity")]
    public float DashDistance = 6f;

    [Tooltip("Czas trwania dasza w sekundach")]
    public float DashDuration = 0.2f;

    [Tooltip("Maksymalna liczba doskoków pod rząd, zanim zostanie nałożony pełny cooldown.")]
    public int MaxConsecutiveDashes = 0;

    [Tooltip("Czas odczekania po daszu, zanim można wykonać kolejny")]
    public float DashCooldown = 0.3f;


    private bool _canDash = true;
    private int _currentConsecutiveDashes = 0;

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
        if (!_enabled || !_canDash) return;


        if (_player.IsClimbing && _currentConsecutiveDashes > 0)
        {
            ResetDashChain();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
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
        if (_currentConsecutiveDashes < MaxConsecutiveDashes)
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

        if (_currentConsecutiveDashes < MaxConsecutiveDashes)
        {
            float miniCooldown = 0.1f;
            yield return new WaitForSeconds(miniCooldown);

            _canDash = true;
        }
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }
}
