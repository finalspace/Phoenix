using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private float coolDownStartAt;
    private float coolDown = 1.0f;

    private void Start()
    {
        Cooldown();
    }

    private void Update()
    {
        if (Time.time - coolDownStartAt > coolDown)
        {
            Vector2 vel = new Vector2(0, 1.5f + RandomGenerator.GetRandomFloat());
            playerMovement.Flip(RandomGenerator.GetRandomFloat() - 0.5f);
            playerMovement.Launch(vel);
            Cooldown();
        }
    }

    private void Cooldown()
    {
        coolDown = RandomGenerator.GetRandomFloat() * 2;
        coolDownStartAt = Time.time;
    }
}
