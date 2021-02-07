using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFactory : SingletonBehaviour<PlatformFactory>
{
    public List<GameObject> platforms;

    public GameObject GetPlatform(int id)
    {
        if (id < 0 || id >= platforms.Count)
            return null;
        return platforms[id];
    }
}
