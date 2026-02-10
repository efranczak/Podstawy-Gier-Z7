using TMPro;
using UnityEngine;

public class LeaderboardInputLetter : MonoBehaviour
{
    private static char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
    public TMP_Text displayText;


    int currentLetterIndex = 0;


    void Start()
    {

    }

    void Update()
    {
        
    }

    public void IncrementLetter()
    {
        currentLetterIndex++;
        if (currentLetterIndex >= letters.Length)
        {
            currentLetterIndex = 0;
        }
        UpdateDisplay();
    }

    public void DecrementLetter()
    {
        currentLetterIndex--;
        if (currentLetterIndex < 0)
        {
            currentLetterIndex = letters.Length - 1;
        }
        UpdateDisplay();
    }

    public char GetCurrentLetter()
    {
        return letters[currentLetterIndex];
    }


    private void UpdateDisplay()
    {
        displayText.text = letters[currentLetterIndex].ToString();
    }

    public void SetActive()
    {
        displayText.color = Color.yellow; 
    }

    public void SetNotActive()
    {
        displayText.color = Color.white;
    }






}
