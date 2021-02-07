using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoingKit;

public class CameraManager : SingletonBehaviour<CameraManager>
{
    public Transform followingTarget;
    public Camera cam;

    private Vector3 targetPos;
    private bool following = true;

    public BoingBehavior boingCtrl;

    [Header("Debug Shake")]
    public float magnitude = 1.0f;
    public float roughness = 8;
    public float zoomPower = 0f;
    public float sustainTime = 1;
    public float fadeInTime = 0;
    public float fadeOutTime = 0.5f;

    private CameraShakeInstance shakeInstance;

    private void Update()
    {
        if (following)
        {
            targetPos = new Vector3(followingTarget.position.x, followingTarget.position.y, transform.position.z);
        }

        transform.position = targetPos;
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayShake();
        }
    }

    public void StopFollowing(Vector3 lastVel)
    {
        following = false;
        targetPos += lastVel * Time.fixedDeltaTime * 5;
        boingCtrl.Params.PositionOscillationFrequency = 0.5f;
        boingCtrl.Params.PositionOscillationHalfLife = 1f;
    }

    public void PlayShake()
    {
        shakeInstance = new CameraShakeInstance(magnitude, roughness, sustainTime, fadeInTime, fadeOutTime, zoomPower, cam);
        CameraShaker camShaker = cam.gameObject.AddComponent<CameraShaker>();
        camShaker.Init(shakeInstance);
        camShaker.Play();
    }
}
