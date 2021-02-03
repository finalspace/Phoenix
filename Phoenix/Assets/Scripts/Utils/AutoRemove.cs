using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRemove : MonoBehaviour
{
    public float time = 1.0f;
    public bool autoStart = true;

    private void Start()
    {
        if (autoStart)
            Invoke("AutoDestroy", time);
    }

    private void AutoDestroy()
    {
        Destroy(gameObject);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
