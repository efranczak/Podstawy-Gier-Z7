using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Tooltip("Punkt wejscia, do którego dolaczony bedzie poprzedni chunk.")]
    public Transform Entry;

    [Tooltip("Punkt wyjscia, od ktorego dolaczony bedzie nastepny chunk.")]
    public Transform Exit;

    [Tooltip("Decyduje czy chunk moze byc wygenerowany w formie odbitej w osi X.")]
    public bool isFlippable;

    [Tooltip("Poziom trudnosci chunka. Skala 0-10.")]
    public int difficultyLevel;

    [Tooltip("Minimalna (lub zalecana ig) liczba dashy do przejscia chunka.")]
    public int howManyDashes = 0;

    [Tooltip("Minimalna (lub zalecana ig) liczba skokow do przejscia chunka. Jeden to pojedynczy skok, dwa to double jump, etc.")]
    public int howManyJumps = 1;

    [Tooltip("Czy przejscie wymaga uzycia Wall Jump.")]
    public bool wallJumpingRequired = false;

    [Tooltip("Minimalna (lub zalecana ig) liczba odbic od jednej sciany w walljump do przejscia chunka. Jezeli wynosi 0 to znaczy, ze poziom nie wymaga wall jumpa.")]
    public int howManySameWallMaxJumps = 0;
}