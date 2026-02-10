using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public LeaderboardManager leaderboardManager;
    public TMP_Text[] nameTexts;
    public TMP_Text[] scoreTexts;
    public TMP_Text currentScore;
    public Animator animator;
    public TMP_Text titleText;
    public LeaderboardInput input;

    private bool isLeaderboardActive = false;
    private bool animationPlayed = false;

    private bool BackgroundDisplayed = false;
    private bool TitleDisplayed = false;
    private bool ScoresDisplayed = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leaderboardPanel.SetActive(false);
        titleText.gameObject.SetActive(false);
        input.gameObject.SetActive(false);

        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(false);
            if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateLeaderboard()
    {
        StartCoroutine(PlayAnimationSequence());
    }

    private IEnumerator PlayAnimationSequence()
    {
        leaderboardPanel.SetActive(true);

        Debug.Log("Activating leaderboard animation sequence.");

        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.SetTrigger("showAnim");
            Debug.Log("Started leaderboard animation.");
        }

        yield return new WaitForSecondsRealtime(2.0f);



        titleText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f); 

        // Wyœwietl wyniki
        UpdateLeaderboardUI();

        for (int i = 0; i < nameTexts.Length; i++)
        {
            if (nameTexts[i] != null) nameTexts[i].gameObject.SetActive(true);
            if (scoreTexts[i] != null) scoreTexts[i].gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f); 
        }

        yield return new WaitForSecondsRealtime(0.5f);
        input.gameObject.SetActive(true);
        if (currentScore != null && ScoreManager.instance != null)
        {
            currentScore.gameObject.SetActive(true);
            currentScore.text = ScoreManager.instance.GetScore().ToString();
        }


    }


    private void UpdateLeaderboardUI()
    {
        List<ScoreEntry> highScores =
            (leaderboardManager != null)
            ? leaderboardManager.GetScores()
            : new List<ScoreEntry>();

        int len = Mathf.Min(nameTexts.Length, scoreTexts.Length);

        for (int i = 0; i < len; i++)
        {
            if (i < highScores.Count)
            {
                nameTexts[i].text = (highScores[i].name.Length>3 ?   "AAA" : highScores[i].name) + "...............................................";
                scoreTexts[i].text = highScores[i].score.ToString();
            }
            else
            {
                nameTexts[i].text = "---" + "..................................................";
                scoreTexts[i].text = "0";
            }
        }
    }


    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
