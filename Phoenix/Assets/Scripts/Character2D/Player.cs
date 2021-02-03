using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonBehaviour<Player>
{
    //public CharacterSpineAnimator animator;
    public PlayerMovement playerMovement;

    private PlayerStats playerStats;
    private PlayerCollision playercollision;

    private bool isSimulating = true;
    private bool isTakingControl = true;

    [Header("Aiming Effects")]
    public GameObject aimingRoot;
    public List<GameObject> aimingDots;
    public int aimingDotsCount = 6;
    public bool fullTrajctory = false;

    private bool aiming = false;
    private float aimingTime;
    private Vector3 camFirstPos;
    private Vector3 mouseFirstPos;
    private Vector3 mousePosition;
    public Vector3 aimingPosOffset = new Vector3(0, 0.5f, 0);  //offset so you already aiming up at start
    private bool buttonPressed = false;

    [Header("Respawn")]
    public Vector3 respawnPosOffset;

    [Header("Debug")]
    public bool debugNoDie = false;

    private void OnEnable()
    {
        //EventManager.OnPlayerLand += OnPlayerLand;
    }

    private void OnDisable()
    {
        //EventManager.OnPlayerLand -= OnPlayerLand;
    }

    private void Start()
    {
        playerStats = PlayerStats.Instance;
        //animator = GetComponent<CharacterSpineAnimator>();
        playerMovement = GetComponent<PlayerMovement>();
        playercollision = GetComponent<PlayerCollision>();

        aimingRoot.transform.SetParent(null);
        if (fullTrajctory)
            ShowFullTrajectory();
    }

    public void Update()
    {
        if (isTakingControl)
            HandleInput();

        if (aiming)
            DrawTrajectory();

        // die if player height goes below the starting level; can update the fatal height as we got
        if (transform.position.y < playerStats.fatalHeightFalling)
        {
            Kill();
        }

        //HandleInteraction();
    }

    //need non-trigger collider
    private void HandleInteraction()
    {
        /*
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, interactable);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("Fuel"))
            {
                hitInfo.collider.GetComponent<Fuel>().Collect();
            }

            if (hitInfo.collider.CompareTag("Cloud"))
            {
            }

            if (hitInfo.collider.CompareTag("Moth"))
            {
            }
        }
        */
    }

    private void ShowFullTrajectory()
    {
        for (int i = 0; i < aimingDots.Count - 1; ++i)
        {
            aimingDots[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void DrawTrajectory()
    {
        Vector2 startPos = transform.position + aimingPosOffset;
        Vector2 vel = ComputeInitialVelocity();
        Vector2[] dotPosition = playerMovement.GetTrajectory(startPos, vel, aimingTime);
        for (int i = 0; i < aimingDotsCount; ++i)
        {
            aimingDots[i].transform.position = dotPosition[i];
        }
    }

    /// <summary>
    /// calculate launch velociy based on drag input
    /// </summary>
    /// <returns></returns>
    private Vector2 ComputeInitialVelocity()
    {
        Vector2 power;
        Vector2 diff = (mouseFirstPos - mousePosition) + aimingPosOffset;
        float x = Mathf.InverseLerp(0, 6, Mathf.Abs(diff.x));
        //x = Mathf.Sqrt(x);  //slow start
        x = Mathf.Sin(x * Mathf.PI / 2);  //fast start

        float y = Mathf.InverseLerp(0, 1.5f, Mathf.Abs(diff.y));
        y = Mathf.Sqrt(y);

        power = new Vector2(x * Mathf.Sign(diff.x), y * Mathf.Sign(diff.y));
        Vector2 vel = playerMovement.GetMappedVelocity(power);
        return vel;
    }

    public void SetAiming(bool val)
    {
        if (aiming == val)
            return;
        aiming = val;

        aimingRoot.SetActive(val);
        if (!aiming)
        {
            aimingTime = 0;
        }

        /*
        if (aiming)
            animator.PlaySquish();
        else
            animator.PlayIdle();
        */
    }



    /*****************************************
     * 
     * Actions
     * 
     *****************************************/
    /// <summary>
    /// Handles the input. 
    /// calculate deltaMovement used for UpdateMovement
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buttonPressed = true;
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseFirstPos = mousePosition;
        }

        if (buttonPressed && playerMovement.AbleToJump())
        {
            SetAiming(true);

            /*
            //slow motion aiming?
            if (playerMovement.dashReady)
                TimeManager.Instance.SlowMotion();
                */
        }

        if (Input.GetMouseButton(0))
        {
            if (!aiming) return;
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            buttonPressed = false;
            if (!aiming) return;

            SetAiming(false);
            TimeManager.Instance.Reset();

            if (Vector2.Distance(mousePosition, mouseFirstPos) > 0.01f)
                playerMovement.Launch(ComputeInitialVelocity());
            else playerMovement.LaunchFailed();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TimeManager.Instance.TogglePauseGame();
        }
    }

    public void CollectItem()
    {
        MusicManager.Instance?.PlayEat();
        //animator.PlayEat();
    }

    /// <summary>
    /// Damage()->Kill()->Die()
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage = 0)
    {
        if (!isSimulating || playerStats.IsInvulnerable || playerMovement.isDashing) return;

        MusicManager.Instance?.PlayDamage();
        //animator.PlayHurt();

        //todo: damage system to trigger Kill/Die
        Die();
    }

    public void Kill()
    {
        if (!isSimulating || playerStats.IsInvulnerable) return;

        Die();
    }

    /// <summary>
    /// set to dying status. allow some last chances to survive
    /// i.e. character running out of energy in the air, but will survive if he lands on a fuel or campfire
    /// </summary>
    public void SoftKill()
    {
		if (!isSimulating || playerStats.IsInvulnerable) return;

		if (playerMovement.isOnGround)
        {
            Die();
            return;
        }

        playerStats.isDying = true;
    }

    private void Die()
    {
        if (debugNoDie) return;

        playerStats.lives--;
        UIManager.Instance?.UpdateLife(playerStats.lives);
        if (playerStats.lives <= 0)
        {
            MainGameManager.Instance.GameLost();
            Destroy(gameObject);
        }
        else
        {
            Invoke("Respawn", 2);
        }

        SetAiming(false);
        playerStats.isDying = false;
        //animator.PlayDie();

        StopSimulation();
    }

    public void Respawn()
    {
        MusicManager.Instance?.PlayThunder();
        playerStats.energy = 100;
        playerMovement.Reset();
        transform.position = GetRespawnPosition();
        //animator.PlayBirth();
        SetInvulnerable();
        Invoke("DismissInvulnerable", 3);

        StartSimulation();
    }

    private Vector3 GetRespawnPosition()
    {
        Vector3 respawnPos = Vector3.zero;
        /*
        if (BoardManager.Instance != null)
        {
            BoardManager brd = BoardManager.Instance;
            GameObject checkPoint = brd.GetCurrentWaypoint();
            respawnPos = checkPoint.transform.position;
        }
        else
        {
            //respawnPos = CameraManager.Instance.transform.position + 2f * Vector3.up;
            respawnPos = CameraManager.Instance.transform.position + respawnPosOffset;
            respawnPos.z = 0;
        }
        */

        // put player a little higher than achieved waypoint or will fall through
        respawnPos += 1.3f * Vector3.up;

        
        return respawnPos;
    }

    public void CenterOnWaypoint()
    {
        // respawn at highest waypoint reached, showing corresponding number in log
        //Debug.Log("Respawning at waypoint " + PlayerStats.Instance.highestWaypoint);
        //PlayerMovement move = GameObject.FindObjectOfType<PlayerMovement>();
        // if currently aiming, release that
        //move.SetAiming(false);
        // go to the vector
        //move.GoToHighestWaypoint();
    }

    public void GrantDash()
    {
        if (!isSimulating) return;

        playerMovement.dashReady = true;
        TimeManager.Instance.SlowMotion();
    }

    /*****************************************
     * 
     * Status
     * 
     *****************************************/
    /// <summary>
    /// character doesn't react to game systems (movement, input control)
    /// used for cutscene, or play die animation
    /// </summary>
    public void StopSimulation()
    {
        isSimulating = false;
        isTakingControl = false;
        playerMovement.StopSimulation();
    }

    public void StartSimulation()
    {
        isSimulating = true;
        isTakingControl = true;
        playerMovement.StartSimulation();
    }

    public void StartControl()
    {
        isTakingControl = true;
    }

    public void StopControl()
    {
        isTakingControl = false;
    }

    public void SetInvulnerable()
    {
        //animator.PlayFlash();
        playerStats.IsInvulnerable = true;
        Invoke("DismissInvulnerable", 3);
    }

    public void DismissInvulnerable()
    {
        //animator.StopFlash();
        playerStats.IsInvulnerable = false;
    }

    /*****************************************
     * 
     * Events
     * 
     *****************************************/
    private void OnPlayerLand()
    {
        if (playerStats.isDying)
            Kill();
    }
}
