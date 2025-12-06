using UnityEngine;

public class PlatfromerApple : MonoBehaviour
{   
    private BoxCollider2D boxCollider;
    public bool isCollected = false;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            gameObject.SetActive(false);
        }
    }
}
