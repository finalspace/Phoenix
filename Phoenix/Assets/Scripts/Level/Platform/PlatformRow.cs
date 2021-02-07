using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRow : MonoBehaviour
{
    public PlatformRow bottomRow;
    public PlatformRow topRow;
    public Transform[] spots;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            int[] data = new int[11];
            Spawn(data);
        }
    }

    public void Spawn(int[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            GameObject prefab = PlatformFactory.Instance.GetPlatform(data[i]);
            if (prefab != null)
            {
                GameObject platformObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                platformObj.transform.SetParent(spots[i], false);
            }
        }
    }


}
