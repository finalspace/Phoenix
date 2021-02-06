using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {
    public static CameraShaker Instance;

    CameraShakeInstance shakeInstance;

    public Action callback;

    private Vector3 oldPos;
    private Vector3 oldRotation;
    private Vector3 posAddShake, rotAddShake;
    private bool playing;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!playing)
            return;

        posAddShake = Vector3.zero;
        rotAddShake = Vector3.zero;

        if (shakeInstance.CurrentState == CameraShakeState.Inactive)
        {
            OnComplete();
            return;
        }
        else
        {
            posAddShake += MultiplyVectors(shakeInstance.UpdateShake(), shakeInstance.PositionInfluence);
            rotAddShake += MultiplyVectors(shakeInstance.UpdateShake(), shakeInstance.RotationInfluence);
        }

        transform.localPosition = new Vector3((posAddShake + oldPos).x, (posAddShake + oldPos).y, transform.localPosition.z);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, (rotAddShake + oldRotation).z);

    }

    public void Init(CameraShakeInstance shakeInstance = null, Action callback = null)
    {
        this.shakeInstance = shakeInstance;
        if (this.shakeInstance == null)
            GenerateDefaultShakeInstance();
        this.callback = callback;
    }

    public void Play(Action callback = null)
    {
        if (playing)
            return;

        if (shakeInstance == null)
            GenerateDefaultShakeInstance();

        this.callback = callback;

        oldPos = Vector3.zero;
        oldRotation = Vector3.zero;
        playing = true;
    }

    private void GenerateDefaultShakeInstance()
    {
        shakeInstance = new CameraShakeInstance(1, 8, 1, 0, 0.5f, 0.2f, Camera.main);
    }

    public void OnComplete()
    {
        transform.localPosition = oldPos;
        transform.localEulerAngles = oldRotation;
        shakeInstance = null;
        playing = false;

        if (callback != null)
            callback();

        Destroy(this);
    }

    /// <summary>
    /// Multiplies each element in Vector3 v by the corresponding element of w.
    /// </summary>
    public static Vector3 MultiplyVectors(Vector3 v, Vector3 w)
    {
        v.x *= w.x;
        v.y *= w.y;
        v.z *= w.z;

        return v;
    }
}
