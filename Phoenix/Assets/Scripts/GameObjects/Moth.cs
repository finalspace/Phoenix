using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moth : MonoBehaviour
{
    private float energy = 50;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        Collect();
    }

    public void Collect()
    {
        PlayerStats.Instance.UpdateEnergy(energy);
        Player.Instance.CollectItem();
        Destroy(transform.parent.gameObject);
    }
}
