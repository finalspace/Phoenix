using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private float energy = -20;
    private float cooldown = 2;
    private float timer = 0.0f;
    private bool active = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        Touch();
    }

    private void Update()
    {
        if (active) return;

        timer += Time.deltaTime;
        if (timer > cooldown)
        {
            timer = 0;
            active = true;
        }
    }

    public void Touch()
    {
        if (!active) return;
        timer = 0;

        PlayerStats.Instance.UpdateEnergy(energy);
        Player.Instance.Damage();
    }
}
