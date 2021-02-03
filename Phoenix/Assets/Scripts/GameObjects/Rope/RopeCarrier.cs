using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// handle grab rope behavior
/// 1) disable character default movement
/// 2) slide to the end the rope over time, then start swinging
/// 3) disable rope swing after is released
/// </summary>
public class RopeCarrier : MonoBehaviour
{
    private RopeControl rope;
    private int current;
    private int end;
    private Transform target;
    private float speed = 4f;  //in FixedUpdate
    private float dealthMovement = 0.22f;  //in FixedUpdate
    private bool sliding = false;  //before character reaches the end of the rope
    private Vector3[] lastPos;

    //temporary variables
    private float dist;
    private Vector3 direction;
    private float moveLength;
    private float moreToGo;  //extra distance to go after hit the current target

    private void OnEnable()
    {
        EventManager.OnPlayerJump += Release;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerJump -= Release;
    }

    void FixedUpdate()
    {
        if (sliding)
        {
            dist = (target.position - transform.position).magnitude;
            direction = (target.position - transform.position).normalized;
            moveLength = Mathf.Min(dist, dealthMovement);

            transform.position += direction * moveLength;
            moreToGo = dealthMovement - moveLength;

            //(dist - moveLength) < speed * Time.fixedDeltaTime / 4
            if (moreToGo > 0)
            {
                if (UpdateTarget())
                {
                    direction = (target.position - transform.position).normalized;
                    transform.position += direction * moreToGo;
                }
            }

            transform.rotation = target.rotation;
        }
        RecordLastPositions();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ropeControl"></param>
    /// <param name="idx">idx of rope segment</param>
    /// <param name="vel">initial velocity</param>
    public void Grab(RopeControl ropeControl, int idx, Vector3 vel)
    {
        rope = ropeControl;
        current = idx;
        end = ropeControl.total;
        target = rope.segments[current + 1].transform;
        transform.SetParent(rope.segments[current].transform, true);
        lastPos = new Vector3[3];
        RecordLastPositions();

        rope.rope.Drag(current, vel/3);
        sliding = true;
    }

    public void Release(Vector3 vel)
    {
        rope.rope.Drag(current, -vel/2);
        rope.Release();
        Destroy(this);
    }

    private void RecordLastPositions()
    {
        for (int i = 0; i < 3; i++)
        {
            lastPos[i] = rope.segments[end - 1 - i].transform.position;
        }
    }

    private bool UpdateTarget()
    {
        current++;
        if (current == end - 1)
        {
            //move it to target
            transform.SetParent(target);
            transform.position = rope.segments[end - 1].transform.position;

            sliding = false;
            //int dir = (target.transform.position.x > rope.transform.position.x) ? 1 : -1;
            float diff = 0;
            for (int i = 0; i < 3; i++)
            {
                diff += rope.segments[end - 1 - i].transform.position.x - lastPos[i].x;
            }
            int dir = diff > 0 ? 1 : -1;
            rope.StartSwing(dir);
            return false;
        }
        else
        {
            transform.SetParent(rope.segments[current].transform, true);
            target = rope.segments[current + 1].transform;
            return true;
        }
    }
}
