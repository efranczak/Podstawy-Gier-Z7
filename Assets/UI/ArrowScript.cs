using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    private bool isBlinking = false;
    public float blinkInterval = 0.5f;


    public void ChangeState()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }


    public void StartBlinking()
    {   
        if (isBlinking) return;
        InvokeRepeating("ChangeState", 1f, blinkInterval);
        isBlinking = true;
    }

    public void StopBlinking()
    {
        CancelInvoke("ChangeState");
        gameObject.SetActive(true);
        isBlinking = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }



}
