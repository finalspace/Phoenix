using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{
    public static bool Test(float percentage)
    {
        float val = Random.Range(0, 1f);
        return val <= percentage;
    }

    public static float GetRandomFloat()
    {
        float val = Random.Range(0, 1f);
        return val;
    }
}
