using UnityEngine;

public class LianaUsage : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRigidbody;

    private float vertical;
    private float speed = 5f;
    private bool isLiana;
    private bool isClimbing;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Vertical");

        if (isLiana && Mathf.Abs(vertical) > 0.1f)
        {
            isClimbing = true;
        }

    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            playerRigidbody.gravityScale = 0f;
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, vertical * speed);
        }
        else
        {
            playerRigidbody.gravityScale = 4f;
        }
    }



    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Liana"))
        {
            isLiana = true;
        }

    }

    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Liana"))
        {
            isLiana = false;
            isClimbing = false;
        }
    }

}
