using UnityEngine;
using System.Collections;

public class DashHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private bool _enabled = false;

    [Header("Dash Settings")]
    [Tooltip("Jak daleko gracz dashuje w jednostkach Unity")]
    public float DashDistance = 6f;

    [Tooltip("Czas trwania dasza w sekundach")]
    public float DashDuration = 0.2f;

    [Tooltip("Czas odczekania po daszu, zanim można wykonać kolejny")]
    public float DashCooldown = 0.8f;

    private bool _canDash = true;

    private void Update()
    {
        if (!_enabled || !_canDash) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }
    }

    private void TryDash()
    {
        // mozna pozniej tu dodac warunki gdy dash nie moze byc wykonany

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        _canDash = false;

        float direction = _player.GetFacingDirection();

        // speed = distance / duration
        Vector2 dashVelocity = new Vector2(direction * (DashDistance / DashDuration), 0f);

        _player.AddExternalVelocity(dashVelocity, DashDuration);

        // waiting for dash to finish
        yield return new WaitForSeconds(DashDuration + DashCooldown);

        _canDash = true;
    }

    public void SetEnabled(bool value)
    {
        _enabled = value;
    }
}
