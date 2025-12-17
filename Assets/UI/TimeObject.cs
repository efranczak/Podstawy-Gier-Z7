using UnityEngine;

public class TimeObject : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider;
    private TimeHandler _handler;
    [SerializeField] private float _timePower;

    private bool isCollected = false;

    void Start()
    {
        _handler = GameObject.FindGameObjectsWithTag("TimeHandler")[0].GetComponent<TimeHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            _handler.addTime(_timePower);
            gameObject.SetActive(false);
        }
    }
}
