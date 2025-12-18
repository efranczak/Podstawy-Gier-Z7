using UnityEngine;
using System;

public class PlayerSkills : MonoBehaviour
{
    [Header("Generator Requirements")]
    [Tooltip("Aktualny poziom trudnosci gry.")]
    [SerializeField] private int _currentDifficulty = 0;

    [Header("Player Abilities")]
    [Tooltip("Aktualna maksymalna liczba skokow.")]
    [SerializeField] public int _playerJumps = 1;

    [Tooltip("Aktualna liczba dostepnych dashy.")]
    [SerializeField] public int _playerDashes = 0;

    [Tooltip("Aktualna ilosc wall jumpow na tej samej scianie.")]
    [SerializeField] public int _sameWallJumpMaxAmount = 0;

    public int CurrentDifficulty => _currentDifficulty;
    public int PlayerJumps => _playerJumps;
    public int PlayerDashes => _playerDashes;
    public int SameWallJumpMaxAmount => _sameWallJumpMaxAmount;

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

    public void SetSameWallJumpMaxAmount(int amount)
    {
        _sameWallJumpMaxAmount = amount;
        Debug.Log($"Same Wall Jump Max Amount set to {_sameWallJumpMaxAmount}");
    }
}