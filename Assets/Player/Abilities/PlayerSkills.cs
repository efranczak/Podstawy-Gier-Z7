using UnityEngine;
using System;

public class PlayerSkills : MonoBehaviour
{
    [Header("Generator Requirements")]
    [Tooltip("Aktualny poziom trudnosci gry.")]
    [SerializeField] private int _currentDifficulty = 0;

    [Header("Player Abilities")]
    [Tooltip("Aktualna maksymalna liczba skokow.")]
    [SerializeField] private int _playerJumps = 1;

    [Tooltip("Aktualna liczba dostepnych dashy.")]
    [SerializeField] private int _playerDashes = 0;

    [Tooltip("Czy gracz ma Wall Jump.")]
    [SerializeField] private bool _playerHasWallJump = false;

    public int CurrentDifficulty => _currentDifficulty;
    public int PlayerJumps => _playerJumps;
    public int PlayerDashes => _playerDashes;
    public bool PlayerHasWallJump => _playerHasWallJump;

    public void IncreaseDifficulty(int amount)
    {
        _currentDifficulty += amount;
        Debug.Log($"Difficulty increased to {_currentDifficulty}");
    }

    public void SetMaxJumps(int count)
    {
        if (count > _playerJumps)
        {
            _playerJumps = count;
            Debug.Log($"Max Jumps set to {_playerJumps}");
        }
    }
    public void SetMaxDashes(int count)
    {
        _playerDashes = count;
        Debug.Log($"Max Dashes increased to {_playerDashes}");
    }

    public void SetWallJumpStatus(bool enabled = true)
    {
        _playerHasWallJump = enabled;
        Debug.Log($"Wall Jump set to {_playerHasWallJump}");
    }
}