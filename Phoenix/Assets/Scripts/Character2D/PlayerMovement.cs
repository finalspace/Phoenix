using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using C1.Feedbacks;

[RequireComponent(typeof(PlayerCollision))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Moving and Jumping")]
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    public float jumpHorizontalPower = 15;
    public Transform root;    //view root

    public PlayerCollision playerCollision;
    private Vector2 deltaMovement;
    private float accelerationTimeAirborne = .9f;
    private float accelerationTimeGrounded = .2f;
    private float gravity;
    private float jumpVelY;
    public Vector2 velocity;
    public float targetVelocityX;  //don't set x velocity directly
    public Vector2 landingTarget;
    private bool homingLandingTarget = false;

    private float velocityXSmoothing;
    private float velXSmoothingTemp;

    [Header("Status")]
    private bool isSimulating = true;
    private bool isDead = false;
	private bool facingRight = true;
    private bool doubleJump = false;
    private bool isRunning = false;
    public bool isOnGround = false;
    public bool isJumping = true;
    //private bool isOnRope = false;
    const int maxJumpNum = 1;
    public int jumpNum;
    public bool isDashing = false;
    public bool dashReady = false;
    private bool isWalling = false;
    private bool tiltSliding = false;

    [Header("Visual Effects")]
    public C1Feedbacks landingFeedbacks;
    public GameObject damageEffect;
	public GameObject footEffect;
    public GameObject jumpFX;
    public GameObject dashFX;
	public Animator hurtPanel;
    public GameObject trailEffect;
    public float startTrailEffectTime;
    private float trailEffectTime;

    void Start() {
        playerCollision = GetComponent<PlayerCollision> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        jumpVelY = Mathf.Abs(gravity) * timeToJumpApex;
        jumpNum = maxJumpNum;
        dashReady = false;
    }

    private void FixedUpdate()
    {
        if (isSimulating)
        {
            SimulateMovement();
            HandlePhysics();
        }
        else
        {
            SimulateMovementOnly();
        }
    }

    /// <summary>
    /// Update velocity and simulate movement
    /// Based on Time (velocity accumulation, position change)
    /// Physics data ready after this step
    /// </summary>
    private void SimulateMovement()
    {
        //Flip(deltaMovement);

        //------------------------ Update Velocity ---------------------------------------
        //------------------------                 ---------------------------------------
        //float targetVelocityX = deltaMovement.x * moveSpeed;  //keyboard direct control
        float accelerationTime = isOnGround ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime, Mathf.Infinity, Time.fixedDeltaTime);

        //velocity.y
        if (isDashing)
        {
            //no gravity
            velocity *= 0.9f;
            if (velocity.magnitude < 5.0f)
            {
                targetVelocityX = velocity.x;
                isDashing = false;
            }
        }
        else if (isWalling)
        {
            //remain slow speed
        }
        else
        {
            //apply gravity
            velocity.y += gravity * Time.fixedDeltaTime;
            if (velocity.y < 0)
            {
                velocity.y += gravity * Time.fixedDeltaTime;
                if (homingLandingTarget)
                {
                    LandingHoming();
                }
            }
        }

        //------------------------ Update Position ---------------------------------------
        //------------------------                 ---------------------------------------
        playerCollision.Move(velocity * Time.fixedDeltaTime);  //move and update physics status based on new position
    }

    private void SimulateMovementOnly()
    {
        //------------------------ Update Velocity ---------------------------------------
        //------------------------                 ---------------------------------------
        float accelerationTime = isOnGround ? accelerationTimeGrounded : accelerationTimeAirborne;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime, Mathf.Infinity, Time.fixedDeltaTime);
        velocity.y += gravity * Time.fixedDeltaTime;

        //------------------------ Update Position ---------------------------------------
        //------------------------                 ---------------------------------------
        playerCollision.MoveIgnoreCollision(velocity * Time.fixedDeltaTime);  //move and update physics status based on new position
    }

    /// <summary>
    /// Handle physics after movement
    /// update velocity and position
    /// Based on physics
    ///
    /// in this case, position physics is already handled in SimulateMovement();
    /// </summary>
    private void HandlePhysics()
    {
        //------------------------ Update Velocity ---------------------------------------
        //------------------------                 -------------------------------------
        if (playerCollision.collisions.below)
        {
            if (isJumping)
            {
                Land(playerCollision.collisions.belowTransform);
            }

            OnGround();
        }
        else
        {
            isJumping = true;
            float ang = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90;
            //ang = Mathf.LerpAngle(transform.eulerAngles.z, ang, Time.deltaTime * rotateSpeed);
            //root.rotation = Quaternion.Euler(0, 0, ang);
            Debug.DrawRay(transform.position, velocity, Color.green);
        }

        //when blocked by ceiling
        if (playerCollision.collisions.above)
        {
            Debug.Log("Above");
            velocity.y = -velocity.y * 0.3f;
            velocity.x *= 0.8f;
            targetVelocityX *= 0.8f;
        }

        //side wall
        if (playerCollision.collisions.left || playerCollision.collisions.right)
        {
            float speed = Mathf.Max(1f, Mathf.Abs(velocity.x));

            BounceSurface bounceSurface =
                (playerCollision.collisions.left ? playerCollision.collisions.leftTransform : playerCollision.collisions.rightTransform)
                .GetComponent<BounceSurface>();
            AttachableSurface attachableSurface =
                 (playerCollision.collisions.left ? playerCollision.collisions.leftTransform : playerCollision.collisions.rightTransform)
                .GetComponent<AttachableSurface>();

            if (bounceSurface != null)
            {
                velocity.x = Mathf.Sign(-velocity.x) * speed * bounceSurface.bouncePower;
                velocityXSmoothing *= -bounceSurface.bouncePower;
                targetVelocityX *= -bounceSurface.bouncePower;
            }
            else if (attachableSurface != null)
            {
                Land(attachableSurface.transform);
                isWalling = true;
                velocity.x = velocityXSmoothing = targetVelocityX = 0;
                velocity.y = -1;
            }
            else
            {
                velocity.x *= 0.9f;
            }
        }

        //------------------------ Update Status ---------------------------------------
        //------------------------                 -------------------------------------
        isOnGround = playerCollision.collisions.below;
        isJumping = !isOnGround;

        //------------------------ Update View -------------------------------------
        //------------------------             -------------------------------------
        if (isRunning || !isOnGround)
        {
            //UpdateTrailEffect(); 
        }
    }

    /// <summary>
    /// map [0, 1] inclusive to [0, maxJumpVelocity]
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMappedVelocity(Vector2 input)
    {
        Vector2 vel = new Vector2();
        vel.x = Mathf.Lerp(0, jumpHorizontalPower, Mathf.Abs(input.x)) * Mathf.Sign(input.x);
        vel.y = Mathf.Lerp(0, jumpVelY, Mathf.Abs(input.y)) * Mathf.Sign(input.y);
        return vel;
    }

    public Vector2[] GetTrajectory(Vector2 startPos, Vector2 vel, float aimingTime)
    {
        int aimingDotsCount = 7;
        Vector2[] positions = new Vector2[aimingDotsCount];
        Vector2 dotPos = startPos;

        aimingTime += Time.fixedDeltaTime * (1 / Time.timeScale);
        float offTime = Mathf.Repeat(aimingTime, 1.0f);
        offTime = offTime / 10f;
        float dt = Time.fixedDeltaTime * 5 * (1 / Time.timeScale);

        float targetVx = vel.x / 2;
        vel.x = Mathf.SmoothDamp(vel.x, targetVx, ref velXSmoothingTemp, accelerationTimeAirborne, Mathf.Infinity, offTime);
        vel.y += gravity * offTime;
        dotPos += vel * offTime;

        for (int i = 0; i < aimingDotsCount; ++i)
        {
            positions[i] = dotPos;
            vel.x = Mathf.SmoothDamp(vel.x, targetVx, ref velXSmoothingTemp, accelerationTimeAirborne, Mathf.Infinity, dt);
            vel.y += gravity * dt;
            dotPos += vel * dt;
        }

        return positions;
    }

    public bool AbleToJump()
    {
        return (jumpNum > 0 || dashReady);
    }

    /*
    /// <summary>
    /// Handles the input. 
    /// calculate deltaMovement used for UpdateMovement
    /// </summary>
    private void HandleInput()
    {
        deltaMovement = Vector2.zero;

        if (!isDead)
        {
            //keyboard control
            //deltaMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
    */

    /*****************************************
     * 
     * Actions
     * 
     *****************************************/
    private void Jump()
    {
        isJumping = true;
        velocity.y = jumpVelY;
    }

    public void Land(Transform ground)
    {
        EventManager.PlayerLand(velocity);
        isJumping = false;

        /*
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.6f);
        if (footEffect != null)
            Instantiate(footEffect, pos, Quaternion.identity);
        root.rotation = Quaternion.Euler(0, 0, 0);
        */
        landingFeedbacks?.Play();

        ProcessGroundInfo(ground);
        ResetState();
    }

    private void ProcessGroundInfo(Transform trans)
    {
        /*
        AttachToMe attach = trans.gameObject.GetComponent<AttachToMe>();
        if (attach != null)
            transform.SetParent(trans, true);

        TiltSurface tiltSurface = trans.gameObject.GetComponent<TiltSurface>();
        if (tiltSurface)
            tiltSliding = true;
        */
        PlatformBase platform = trans.GetComponent<PlatformBase>();
        if (platform)
        {
            //transform.SetParent(trans, true);
            platform.ObjLanding(gameObject);
            Player.Instance.platform = platform;
        }
    }

    private void ResetState()
    {
        velocity = Vector2.zero;
        root.rotation = Quaternion.Euler(0, 0, 0);
        targetVelocityX = 0;
        velocityXSmoothing = 0;
        jumpNum = maxJumpNum;
        isDashing = false;
        dashReady = false;
        isWalling = false;
    }

    private void OnGround()
    {
        velocity.y = 0;
        if (tiltSliding)
            ProcessTiltSlide();

        homingLandingTarget = false;
    }

    private void ProcessTiltSlide()
    {
        //Debug.Log("TiltSlide");
        float surfaceAngle = playerCollision.CalculateTiltAngle();
        float ang = Mathf.Tan(surfaceAngle) * Mathf.Rad2Deg;

        //angle critical point to start sliding
        float val = Mathf.InverseLerp(10f, 80f, Mathf.Abs(ang));
        val = Mathf.Sin(val * Mathf.PI / 2);  //modify curve

        Vector2 dir = new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad) * Mathf.Sign(ang), -Mathf.Sin(Mathf.Abs(ang * Mathf.Deg2Rad)));
        velocity = dir.normalized * val * 5.0f;

        //Debug
        //float ang1 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        //UIDebugManager.Instance.Arrow.rotation = Quaternion.Euler(0, 0, ang1);
    }

    /*
    public void Launch(Vector2 vel)
    {
        float energy = PlayerStats.Instance.energy;
        if (energy < 3) return;

        isJumping = true;
        isWalling = false;
        jumpNum--;
        Detach();

        velocity = vel;
        float power = velocity.magnitude / 3;
        float adjustedPower = Mathf.Min(power, energy);
        velocity = velocity * (adjustedPower / power);  //adjusted velocity

        isDashing = dashReady;
        if (isDashing)
            velocity *= 2.5f;

        targetVelocityX = velocity.x / 2;

        EventManager.PlayerJump(velocity);

        Vector2 pos = new Vector2(transform.position.x, transform.position.y - 0.6f);
        if (dashFX != null)
        {
            GameObject dashfx = Instantiate(dashFX, pos, Quaternion.identity);
            dashfx.transform.SetParent(transform, true);
        }
    }
    */

    public void Launch(Vector2 vel)
    {
        homingLandingTarget = false;
        Launch(vel, Vector2.zero);
    }
    public void Launch(Vector2 vel, Vector2 landingTgt)
    {
        jumpNum--;
        isJumping = true;
        isWalling = false;
        velocity = vel;
        landingTarget = landingTgt;
        homingLandingTarget = true;

        Detach();
        EventManager.PlayerJump(velocity);
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayJump(vel);
        }
    }

    public void LaunchFailed()
    {
        EventManager.PlayerJumpFail();
    }

    private void LandingHoming()
    {
        float ff = 2 * Mathf.Max(transform.position.y - landingTarget.y, 0) / Mathf.Abs(gravity);
        float landingTime = Mathf.Sqrt(ff);
        if (landingTime > 0.05f)
        {
            targetVelocityX = ((landingTarget.x - transform.position.x) / landingTime);

            float accelerationTime = 0.1f;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime, Mathf.Infinity, Time.fixedDeltaTime);

        }
    }

    void UpdateTrailEffect()
    {
        if (trailEffectTime <= 0)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y - 1f);
            Instantiate(trailEffect, pos, Quaternion.identity);
            trailEffectTime = startTrailEffectTime;
        }
        else
        {
            trailEffectTime -= Time.deltaTime;
        }
    }


	// flip the character so he is facing the direction he is moving in
	void Flip(Vector2 input)
    {
        /*
		if(input.x > 0 && facingRight == false || input.x < 0 && facingRight == true){
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
        */
        Vector3 theScale = transform.localScale;
        theScale.x = (input.x > 0) ? 1 : -1;
        transform.localScale = theScale;
    }

    public void Flip(float velX)
    {
        Vector3 theScale = transform.localScale;
        theScale.x = (velX > 0) ? 1 : -1;
        transform.localScale = theScale;
    }

    public void Detach()
    {
        PlatformBase platform = transform.parent?.GetComponent<PlatformBase>();
        platform?.ObjLeaving();
        transform.SetParent(null);
    }

	public void Damage(){
        if (hurtPanel != null)
            hurtPanel.SetTrigger("Hurt");
        if (damageEffect != null)
            Instantiate(damageEffect, transform.position, Quaternion.identity);
	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Destroy(gameObject);
        }
    }

    /*****************************************
     * 
     * Status
     * 
     *****************************************/
    public void StartSimulation()
    {
        isSimulating = true;

        //rotation might have been modified when out of control
        //update root rotation instead to reflect the change
        root.localRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
    }

    public void StopSimulation()
    {
        isSimulating = false;
        //Reset();
    }

    public void Reset()
    {
        root.rotation = Quaternion.Euler(0, 0, 0);
        velocity = Vector2.zero;
        Detach();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }



}
