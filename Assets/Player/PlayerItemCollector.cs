using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    public float minSpeed = 15f;
    public float maxSpeed = 40f;
    public float maxRange = 12f;

    private GameObject itemInRange;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
            itemInRange = other.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Collectible") && itemInRange == other.gameObject)
            itemInRange = null;
    }

    void Update()
    {
        if (itemInRange != null && Input.GetKeyDown(KeyCode.E))
            ThrowApple(itemInRange);
    }

    void ThrowApple(GameObject apple)
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Snake");
        if (enemy == null) return;

        Rigidbody2D rb = apple.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        Vector2 start = apple.transform.position;
        Vector2 target = enemy.transform.position + Vector3.up * 2f;

        Vector2 dir = target - start;

        if (dir.magnitude > maxRange)
            dir = dir.normalized * maxRange;

        float t = Mathf.Clamp01(dir.magnitude / maxRange);
        float speed = Mathf.Lerp(minSpeed, maxSpeed, t);

        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float dx = dir.x;
        float dy = dir.y;

        float vx = dx / Mathf.Sqrt((2 * dy + Mathf.Sqrt(4 * dy * dy + 4 * gravity * dx * dx / (speed * speed))) / gravity);
        float vy = (dy + 0.5f * gravity * (dx / vx) * (dx / vx)) / (dx / vx);

        rb.linearVelocity = new Vector2(vx, vy);

        itemInRange = null;
    }

}
