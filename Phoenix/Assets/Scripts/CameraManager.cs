using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonBehaviour<CameraManager>
{
    public Camera cam;
    public float magnitude = 1.0f;
    public float roughness = 8;
    public float sustainTime = 1;
    public float fadeInTime = 0;
    public float fadeOutTime = 0.5f;

    private CameraShakeInstance shakeInstance;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayShake();
        }
    }

    public void PlayShake()
    {
        shakeInstance = new CameraShakeInstance(magnitude, roughness, sustainTime, fadeInTime, fadeOutTime, cam);
        CameraShaker camShaker = cam.gameObject.AddComponent<CameraShaker>();
        camShaker.Init(shakeInstance);
        camShaker.Play();
    }
}
