using System.Collections;
using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    [Header("Flow")]
    [SerializeField] private float delaySeconds = 2f;
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject howToPlayPanel;

    private bool _switched;

    private void Start()
    {
        StartCoroutine(SwitchAfterDelay());
    }

    private void Update()
    {
        if (allowSkip && !_switched && (Input.anyKeyDown || Input.GetKeyDown(skipKey)))
        {
            SwitchPanels();
        }
    }

    private IEnumerator SwitchAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        SwitchPanels();
    }

    private void SwitchPanels()
    {
        if (_switched || startPanel == null || howToPlayPanel == null)
        {
            return;
        }

        _switched = true;
        startPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }
}
