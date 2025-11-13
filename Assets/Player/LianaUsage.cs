using UnityEngine;

public class LianaUsage : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    public HingeJoint2D lianaHingeJoint;

    public float pushForce = 5f;
    public bool attachedToLiana = false;
    public Transform attachedTo;
    private GameObject disregard;
    public GameObject pulleySellected = null;

    void Awake()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        lianaHingeJoint = gameObject.GetComponent<HingeJoint2D>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkKeyBoardInputs();
    }

    void checkKeyBoardInputs()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && attachedToLiana)
        {
            Slide(1);
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && attachedToLiana)
        {
            Slide(-1);
        }
    }

    public void Attach(Rigidbody2D lianaBone)
    {
        lianaBone.gameObject.GetComponent<LianaSegment>().isPlayerAttached = true;
        lianaHingeJoint.connectedBody = lianaBone;
        lianaHingeJoint.enabled = true;
        attachedToLiana = true;
        attachedTo = lianaBone.gameObject.transform.parent;
    }

    public void Detach(Rigidbody2D lianaBone)
    {
        lianaBone.gameObject.GetComponent<LianaSegment>().isPlayerAttached = false;
        lianaHingeJoint.connectedBody = null;
        lianaHingeJoint.enabled = false;
        attachedToLiana = false;
    }

    public void Slide(int direction)
    {
        if (attachedToLiana)
        {
            LianaSegment currentSegment = lianaHingeJoint.connectedBody.gameObject.GetComponent<LianaSegment>();
            GameObject newSegment = null;
            if (direction > 0)
            {
                if (currentSegment.connectedAbove != null)
                {
                    if (currentSegment.connectedAbove.gameObject.GetComponent<LianaSegment>() != null)
                    {
                        newSegment = currentSegment.connectedAbove;
                    }
                }
            }
            else
            {
                if (currentSegment.connectBelow != null)
                {
                    newSegment = currentSegment.connectBelow;
                }
            }
            if (newSegment != null)
            {
                transform.position = newSegment.transform.position;
                currentSegment.isPlayerAttached = false;
                newSegment.GetComponent<LianaSegment>().isPlayerAttached = true;
                lianaHingeJoint.connectedBody = newSegment.GetComponent<Rigidbody2D>();
            }
        }
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
       if(!attachedToLiana) {
            if (collision.gameObject.CompareTag("Liana"))
            {
                if (attachedToLiana != collision.gameObject.transform.parent)
                {
                    if (disregard == null || collision.gameObject.transform.parent != disregard)
                    {
                        Attach(collision.gameObject.GetComponent<Rigidbody2D>());
                    }
                }
            }
        }
    }

}
