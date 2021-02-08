using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public GameObject[] spots;

    public void Start()
    {
        Spawn();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            float percentage = 1.0f / (spots[i].transform.childCount * 2);
            for (int j = 0; j < spots[i].transform.childCount; j++)
            {
                if (RandomGenerator.Test(percentage))
                {
                    spots[i].transform.GetChild(j).gameObject.SetActive(true);
                    break;
                }
            }

        }
    }
}
