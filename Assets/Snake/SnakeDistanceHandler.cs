using UnityEngine;

public class SnakeDistanceHandler : MonoBehaviour
{
    [SerializeField] SnakeLogic snakeLogic_;
    [SerializeField] SnakeScript snakeScript_;

    float distance;
    float time;

    bool isPlatfromingSection = false;
    [SerializeField] bool switchToTime = false;

    void Start()
    {
        distance = snakeScript_.DistanceToPlayer();
        time = 0.0f;
    }

    void Update()
    {
        if (isPlatfromingSection && switchToTime) snakeScript_.SetDistanceToPlayer(time*10.0f);
        distance = snakeScript_.DistanceToPlayer();
    }

    public void StartPlatfomingSection()
    {
        time = distance / 10.0f;
        isPlatfromingSection = true;
    }

    public void EndPlatfomingSection()
    {
        isPlatfromingSection = false;
        if (switchToTime) snakeScript_.SetDistanceToPlayer(time*10.0f);
    }
    
    public float GetDistance()
    {
        if (switchToTime && isPlatfromingSection) return time;
        return distance/10.0f;
    }

    public void subtractTime(float t)
    {
        time -= t;
    }
}
