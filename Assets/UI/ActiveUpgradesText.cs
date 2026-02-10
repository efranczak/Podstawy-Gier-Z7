using TMPro;
using UnityEngine;

public class ActiveUpgradesText : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private void ValidateMesh()
    {
        if (_textMesh == null)
        {
            _textMesh = GetComponent<TextMeshProUGUI>();

            if (_textMesh == null)
            {
                Debug.LogError($"[ActiveUpgradesText] Brak TextMeshProUGUI na obiekcie {gameObject.name}!");
            }
        }
    }
    public void SetText(string text)
    {
        ValidateMesh();

        if (_textMesh != null)
        {
            _textMesh.text = text;
            _textMesh.enabled = true;
        }
    }
    public void HideText()
    {
        ValidateMesh();

        if (_textMesh != null)
        {
            _textMesh.enabled = false;
        }
    }
}
