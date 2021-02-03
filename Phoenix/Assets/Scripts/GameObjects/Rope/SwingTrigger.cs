using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not so generic version for rope swing trigger
public class SwingTrigger : MonoBehaviour
{
    public Rope rope;
    public Collider2D zoneCollider;
    //rope segment that reacts to this trigger
    public int ropeSegmentIdx;
    public Vector2 force = new Vector2(0.5f, 0);

    private bool on = false;
    private int direction = 1;
    private bool inZone = false;
    private int count = 0;

    private void FixedUpdate()
    {
        if (!on) return;

        ApplyForce(rope.ropeSegments[ropeSegmentIdx].posNow);
    }

    public void ApplyForce(Vector2 pos)
    {
        if (zoneCollider.bounds.Contains(pos))
        {
            if (!inZone)
            {
                inZone = true;
                direction = -direction;
                count++;
            }

            float coeff = Mathf.Clamp01((float)count / 3);
            rope.Drag(ropeSegmentIdx, force * direction * coeff);
        }
        else inZone = false;
    }

    public void EnableTrigger(int initialDirection)
    {
        direction = initialDirection;
        on = true;
    }

    public void DisableTrigger()
    {
        on = false;
        count = 0;
    }

}
