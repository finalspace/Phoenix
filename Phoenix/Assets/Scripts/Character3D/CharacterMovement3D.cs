using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public class CharacterMovement3D : MonoBehaviour
{
    public CharacterCollision3D charCollision;
    public Transform root;
    public float moveSpeed = 6f;
    public float rotateSpeed = 10f;
    public Vector3 defaultGravity;
    public Animator anim;
    public string animationFloatname;

    private float centerRootOffset = 0;
    private float timeToJumpApex = 0.25f;
    private float jumpVelocity;
    private float jumpMaxTime = .25f;
    private float jumpingMeter;
    private bool inputJumpPressing;
    private bool jumpPressingValid = true;
    private float jumpCountMax = 1;
    private float jumpCount;

    [Header("Input")]
    private Rigidbody rb;
    private float vertical;
    private float horizontal;
    private Vector3 moveDirection;
    private float inputAmount;


    Vector3 raycastFloorPos;
    Vector3 floorMovement;
    Vector3 gravity;
    Vector3 CombinedRaycast;

    [Header("Status")]
    private bool isGrounded;
    private bool isJumping = false;

    private Vector3 vel;

    //update
    private Vector3 inputMovement;


    [Header("Effect")]
    public C1Feedbacks landingFeedback;
    public C1Feedbacks jumpingFeedback;

    //debug
    public Vector3 debugVel;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        gravity = defaultGravity;
        centerRootOffset = transform.position.y - root.position.y;
        jumpVelocity = Mathf.Abs(gravity.y) * timeToJumpApex;
        jumpCount = jumpCountMax;
    }

    // Use this for initialization
    void Start()
    {
    }

    private void Update()
    {
        // reset movement
        moveDirection = Vector3.zero;
        // get vertical and horizontal movement input (controller and WASD/ Arrow Keys)
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpCount++;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpPressingValid = true;
        }
        inputJumpPressing = Input.GetKey(KeyCode.Space);
        if (jumpCount >= jumpCountMax)
        {
            inputJumpPressing = false;
        }

        // base movement on camera
        Vector3 correctedVertical = vertical * Camera.main.transform.forward;
        Vector3 correctedHorizontal = horizontal * Camera.main.transform.right;

        Vector3 combinedInput = correctedHorizontal + correctedVertical;
        // normalize so diagonal movement isnt twice as fast, clear the Y so your character doesnt try to
        // walk into the floor/ sky when your camera isn't level
        moveDirection = new Vector3((combinedInput).normalized.x, 0, (combinedInput).normalized.z);

        // make sure the input doesnt go negative or above 1;
        float inputMagnitude = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        inputAmount = Mathf.Clamp01(inputMagnitude);

        // rotate player to movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(moveDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * inputAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }

        // handle animation blendtree for walking
        //anim.SetFloat(animationFloatname, inputAmount, 0.2f, Time.deltaTime);
    }


    private void FixedUpdate()
    {
        SimulateMovement();
        HandlePhysics();
        debugVel = vel;
    }

    private void SimulateMovement()
    {
        //------------------------ Update Velocity ---------------------------------------
        //------------------------                 ---------------------------------------
        // actual movement of the rigidbody + extra down force
        inputMovement = moveDirection * moveSpeed * inputAmount;
        vel.x = inputMovement.x;
        vel.z = inputMovement.z;
        vel.y += gravity.y * Time.fixedDeltaTime;
        
        //rb.velocity = vel;

        //------------------------ Update Position ---------------------------------------
        //------------------------                 ---------------------------------------
        charCollision.Move(vel * Time.fixedDeltaTime);
        /*
        // find the Y position via raycasts
        floorMovement = new Vector3(rb.position.x, FindFloor().y + centerRootOffset, rb.position.z);

        // only stick to floor when grounded
        if (isGrounded && floorMovement != rb.position)
        {
            // move the rigidbody to the floor
            rb.MovePosition(floorMovement);
        }
        */
    }

    private void HandlePhysics()
    {
        isGrounded = charCollision.collisions.below;

        if (isGrounded)
        {
            if (isJumping)
            {
                Land();
            }
            OnGround();
        }
        else
        {
            if (!isJumping)
            {
                Jump();
            }
            isJumping = true;
        }

        /*
        ///First frame jump
        if (isGrounded && isJumping)
        {
            vel.y = jumpVelocity;
        }
        */

        //pressing jumping
        if (inputJumpPressing && jumpPressingValid)
        {
            vel.y = jumpVelocity;
            jumpingMeter -= Time.fixedDeltaTime;
            if (jumpingMeter < 0)
            {
                jumpPressingValid = false;
            }
        }

        // if not grounded , increase down force
        if (isJumping)
        {
            float cof = (inputJumpPressing && jumpPressingValid) ? 0 : 2.5f;
            if (vel.y < 0)
                cof = 5.0f;

            gravity += defaultGravity * Time.fixedDeltaTime * cof;
        }
    }

    ////////////////////////////
    
    private void Land()
    {
        EventManager.PlayerLand(vel);
        landingFeedback?.Play();
    }

    private void Jump()
    {
        jumpingFeedback?.Play();
    }

    private void OnGround()
    {
        isJumping = false;
        gravity = defaultGravity;
        vel.y = 0;
        jumpingMeter = jumpMaxTime;
        jumpCount = 0;
    }
}
