using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//holds data only
public class PlayerStats : SingletonBehaviour<PlayerStats>
{
    public float maxHeight = 0;
    public int lives = 3;
    public int score = 0;
    public bool losingEnergy = true;
    public float fatalHeightFalling = -14;
    public float energy = 50;
    private float decreasingSpeed = 2;
    public Player player;
    public float startingAltitude = 0;
    public bool isDying = false;

    [Header("status")]
    public bool IsInvulnerable = false;

    private float heightScore = 0;
    private float bonusScore = 0;

    private void OnEnable()
    {
        //EventManager.OnPlayerJump += ConsumeEnergy;
    }

    private void OnDisable()
    {
        //EventManager.OnPlayerJump -= ConsumeEnergy;
    }


    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;

        startingAltitude = player.transform.position.y;
        maxHeight = 0;
        //maxHeight = player.transform.position.y - startingAltitude;
        score = 0;
        heightScore = 0;
        bonusScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (PrototypeManager.Instance.gameState != GameState.Main)
            return;

        if (energy > 0 && losingEnergy)
        {
            energy -= Time.deltaTime * decreasingSpeed;
            if (energy <= 0) player.Kill();
        }

        if (player.transform.position.y - startingAltitude > maxHeight)
        {
            maxHeight = player.transform.position.y - startingAltitude;
            heightScore = maxHeight;
        }
        score = (int)(heightScore * 10 + bonusScore);
        return;

        if (player.transform.position.y > maxHeight)
        {
            maxHeight = player.transform.position.y - startingAltitude;
            score = Mathf.FloorToInt(maxHeight);
        }
        if (energy > 0 && losingEnergy)
        {
            energy -= Time.deltaTime * decreasingSpeed;
            if (energy <= 0) player.SoftKill();
        }

    }

    /*
    public void UpdateEnergy(float value)
    {
        energy += value;
        if (energy < 0)
        {
            energy = 0;
            player.SoftKill();
        }
        energy = Mathf.Clamp(energy, 0, 120);
    }
    */

    public void UpdateEnergy(float value)
    {
        if (value > 0)
            bonusScore += value * 10;

        energy += value;
        if (energy < 0)
        {
            energy = 0;
            player.Kill();
        }
        else if (energy >= 100)
        {
            Player.Instance.TransformDarkPhoenix();
        }
    }

    public void ConsumeEnergy(Vector3 vel)
    {
        float value = vel.magnitude / 3;
        value = 0;
        UpdateEnergy(-value);
    }

}
