using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideRemove : MonoBehaviour
{
    public bool left = true;
    public float leftEdge;
    public bool right = true;
    public float rightEdge;


    // Update is called once per frame
    void Update()
    {
        if ((left && transform.position.x < leftEdge) || (right && transform.position.x > rightEdge))
            Destroy(gameObject);
    }
}
