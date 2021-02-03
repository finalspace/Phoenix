using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegmentGrabble : MonoBehaviour
{
    public RopeControl ropeContrl;
    public int idx;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        Vector3 vel = other.GetComponent<PlayerMovement>().GetVelocity();
        ropeContrl.TryGrab(idx, vel);
    }
}

