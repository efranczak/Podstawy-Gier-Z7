using UnityEngine;

public class TimeHandler : MonoBehaviour
{
    private float totalTime;
    public bool isSubtractingTime = false;

    private void Start()
    {
        totalTime = 0;
    }


    #region Interface
    public void addTime(float time)
    {
        totalTime += time;

    }

    public void subtractTime(float  time)
    {
        if (totalTime - time >= 0) totalTime -= time;
        else totalTime = 0;

    }

    public float getTime()
    {
        return totalTime;
    }

    #endregion

}
