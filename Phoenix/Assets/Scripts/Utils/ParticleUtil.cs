using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleUtil
{
    public static void UpdateParticleColor(GameObject effectObj, Color color)
    {
        ParticleSystem.MainModule main = effectObj.GetComponent<ParticleSystem>().main;
        main.startColor = color;

        foreach (Transform child in effectObj.transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (ps == null)
                continue;

            main = ps.main;
            main.startColor = color;
        }
    }
}
