using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class LandEvent : UnityEvent<Collider2D>
{
}

[RequireComponent (typeof (PlayerCollision))]
public class SimpleMovement : MonoBehaviour
{
	[Header ("Moving and Jumping")]
    public PlayerCollision characterCollision;
	public float gravity;

	public Vector2 velocity;
    public float targetVelocityX;  //don't set x velocity directly
    protected float accelerationTimeAirborne = .9f;
    protected float accelerationTimeGrounded = .2f;
    protected float velocityXSmoothing;
    protected float velXSmoothingTemp;
    protected Collider2D ignoreCollider;

    public UnityEvent landEventSimple = new UnityEvent();
    public LandEvent landEvent;

    [Header("Status")]
    public bool isSimulating = true;
    protected bool isOnGround = false;
    protected bool isJumping = false;

    [Header("Visual Effects")]
	public GameObject footEffect;

    //[Header("-----------------------------")]

    void Start() {
        characterCollision = GetComponent<PlayerCollision> ();
    }

    private void FixedUpdate()
    {
        if (!isSimulating) return;

        SimulateMovement();
        HandlePhysics();
    }

    /// <summary>
    /// Update velocity and simulate movement
    /// Based on Time (velocity accumulation, position change)
    /// Physics data ready after this step
    /// </summary>
    private void SimulateMovement()
    {
        //------------------------ Update Velocity ---------------------------------------
        //------------------------                 ---------------------------------------
        float accelerationTime = isOnGround ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime, Mathf.Infinity, Time.fixedDeltaTime);
        velocity.y += gravity * Time.fixedDeltaTime;

        //------------------------ Update Position ---------------------------------------
        //------------------------                 ---------------------------------------
        characterCollision.MoveIgnoreCollision(velocity * Time.fixedDeltaTime, ignoreCollider);
    }

    /// <summary>
    /// Handle physics after movement
    /// update velocity and position
    /// Based on physics
    ///
    /// in this case, position physics is already handled in SimulateMovement();
    /// </summary>
    public virtual void HandlePhysics()
    {
        //------------------------ Update Velocity ---------------------------------------
        //------------------------                 -------------------------------------
        if (characterCollision.collisions.below)
        {
            if (isJumping == true)
            {
                Land(characterCollision.collisions.belowCollider);
            }

            OnGround();
        }
        else
        {
            isJumping = true;
        }

        //when blocked by ceiling
        if (characterCollision.collisions.above)
        {
            velocity.y = -velocity.y * 0.3f;
            velocity.x *= 0.8f;
            targetVelocityX *= 0.8f;
        }

        //------------------------ Update Status ---------------------------------------
        //------------------------                 -------------------------------------
        isOnGround = characterCollision.collisions.below;
        isJumping = !isOnGround;
    }

    public virtual void Move(Vector2 vel, float targetVelX)
    {
        velocity = vel;
        targetVelocityX = targetVelX;
    }

    public virtual void Land(Collider2D groundCollider)
    {
        isJumping = false;

        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.6f);
        if (footEffect != null)
            Instantiate(footEffect, pos, Quaternion.identity);

        landEvent.Invoke(groundCollider);
        landEventSimple.Invoke();
    }

    public virtual void OnGround()
    {
        velocity.y = 0;
    }

}
