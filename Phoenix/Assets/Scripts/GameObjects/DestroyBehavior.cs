using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyBehavior : MonoBehaviour
{
    public GameObject destroyFX;
    public bool modifyColor = false;
    public Color color;

    private void OnDestroy()
    {
        GameObject fx = Instantiate(destroyFX, transform.position, Quaternion.identity);
        if (modifyColor)
            ParticleUtil.UpdateParticleColor(fx, color);
    }
}
