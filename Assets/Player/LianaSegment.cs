using UnityEngine;

public class LianaSegment : MonoBehaviour
{

    public GameObject connectedAbove, connectBelow;
    public bool isPlayerAttached;

    void Start()
    {
        ResetAnchor();
    }

    //public void ResetAnchor()
    //{
    //    connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
    //    LianaSegment aboveSegment = connectedAbove.GetComponent<LianaSegment>();
    //    if (aboveSegment != null)
    //    {
    //        aboveSegment.connectBelow = gameObject;
    //        float spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
    //        GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom * -1);
    //    }
    //    else
    //    {
    //        GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
    //    }
    // }
    public void ResetAnchor()
    {
        HingeJoint2D joint = GetComponent<HingeJoint2D>();

        if (joint == null)
        {
            Debug.LogError($"[{name}] Missing HingeJoint2D component!");
            return;
        }

        if (joint.connectedBody == null)
        {
            connectedAbove = null;
            GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
            return;
        }

        connectedAbove = joint.connectedBody.gameObject;
        LianaSegment aboveSegment = connectedAbove.GetComponent<LianaSegment>();

        if (aboveSegment != null)
        {
            aboveSegment.connectBelow = gameObject;
            float spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
            joint.connectedAnchor = new Vector2(0, spriteBottom * -1);
        }
        else
        {
            joint.connectedAnchor = Vector2.zero;
        }
    }

}
