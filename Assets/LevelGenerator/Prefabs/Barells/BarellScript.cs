using UnityEngine;

public class BarellScript : MonoBehaviour
{
    public float velocity = 0.1f;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x - velocity, transform.position.y, transform.position.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Zderzenie z graczem
        }
        else
        {   
            // kolizja z czyms innym
            Destroy(gameObject);
        }
    }

    public void SetVelocity(float newVelocity)
    {
        velocity = newVelocity;
    }
}
