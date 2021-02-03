using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]
public class WaypointCollision : MonoBehaviour
{

    private enum WaypointState { Empty, Landed, Bounced, Burning }

    private float landingtime;
    //private float takeofftime;
    private float bouncetime;

    WaypointState state = WaypointState.Empty;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(state == WaypointState.Empty)
            {
                state = WaypointState.Landed;
                landingtime = Time.time;
            }
            //Debug.Log("Energy steady while at camp!");
            // while player is here, energy doesn't run down
            // also we'll replenish it to 100% (tentatively)
            PlayerStats.Instance.losingEnergy = false;
            PlayerStats.Instance.energy = 100;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (state == WaypointState.Bounced && Time.time - landingtime >= 0.5f)
            {
                MusicManager.Instance.PlayCampfire();
                state = WaypointState.Burning;
            }
            else if (state == WaypointState.Landed)
            {
                //Player.Instance.CenterOnWaypoint();  //it breaks the consistency of the world physics
                //bouncetime = Time.time;
                state = WaypointState.Bounced;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (state == WaypointState.Burning)
            {
                MusicManager.Instance.UndampenMusic();
                state = WaypointState.Empty;
            }
            //Debug.Log("Losing energy again! 2D");
            //takeofftime = Time.time;
            PlayerStats.Instance.losingEnergy = true;
        }
    }
}