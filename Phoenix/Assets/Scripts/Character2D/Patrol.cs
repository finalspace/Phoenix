using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
	public float startSpeed;
	public Transform checkPoint;
    public bool movingRight = true;
    public bool manualActivate = false;

    [Header("Return Collider")]
    public string returnColliderTag = "";

    private float speed;
    private bool moving = true;

	void Start()
    {
		Physics2D.queriesStartInColliders = false;
		speed = startSpeed;
        transform.eulerAngles = new Vector3(0, (movingRight ? 0 : 180), 0);
        if (manualActivate)
            moving = false;
    }

    public void Activate()
    {
        moving = true;
    }

	void Update()
    {
        if (!moving) return;

		transform.Translate(Vector2.right * speed * Time.deltaTime);

		RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, 1f);
        if (hit)
        {
            if (!string.IsNullOrEmpty(returnColliderTag))
            {
                if (hit.collider != null && hit.collider.tag == returnColliderTag)
                    ChangeDirection();
            }
        }
        else
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        if (movingRight == false)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            movingRight = false;
        }
    }
}
