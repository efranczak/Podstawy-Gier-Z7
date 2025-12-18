using UnityEngine;
using static UnityEngine.Mathf;

public class ArrowScript : MonoBehaviour
{

    private bool isBlinking = false;
    public float blinkInterval = 0.5f;
    public float scale_scalar = 1f;

    private Vector3 scale = new Vector3(1f, 1f, 1f);
    private bool isActive;


    public void ChangeState()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }


    public void StartBlinking()
    {   
        if (isBlinking || !isActive) return;
        InvokeRepeating("ChangeState", 1f, blinkInterval);
        isBlinking = true;
    }

    public void StopBlinking()
    {
        CancelInvoke("ChangeState");
        gameObject.SetActive(true);
        isBlinking = false;
    }

    public void ChangeSize(float distance)
    {
        gameObject.transform.localScale = scale * Mathf.Sqrt(scale_scalar / distance);
    }

    public void Show()
    {
        if (isBlinking) return;
        gameObject.SetActive(true);
        isActive = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isActive = false;
    }



}
