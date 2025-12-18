using System.Collections;
using UnityEngine;

public class SpriteShading : MonoBehaviour
{ 
    SpriteRenderer sr;
    Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void Flash(Color flashColor, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(flashColor, duration));
    }

    IEnumerator FlashRoutine(Color flashColor, float duration)
    {
        sr.color = flashColor;
        yield return new WaitForSeconds(duration);
        sr.color = originalColor;
    }
}
