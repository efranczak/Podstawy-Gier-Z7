using TMPro;
using UnityEngine;

public class ActiveUpgradesText : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;


    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMesh.enabled = false;
    }

    public void SetText(string text)
    {
        _textMesh.text = text;
        _textMesh.enabled = true;
    }

    public void HideText()
    {
        _textMesh.enabled = false;
    }
}
