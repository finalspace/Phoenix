using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    private bool reached = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        /*
        //Debug.Log("Waypoint!");
        if (!reached && other.tag == "Player")
        {
            // Fire the method that extends the level upwards
            BoardManager lvl = GameObject.FindObjectOfType<LevelManager>().GetComponent<BoardManager>();
            lvl.SetHighestWaypoint(gameObject);
            lvl.WaypointReached();
            //Debug.Log("New waypoint unlocked!");
            reached = true;
        }
        */
    }
}
