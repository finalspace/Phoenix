using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    private Vector3 targetRotation;
    private Action callback;


    public void RotateTo(Vector3 targetRotation, Action callback)
    {
        this.targetRotation = targetRotation;
        this.callback = callback;
        Play(targetRotation, false);
    }

    public void Rotate(Vector3 targetRotation, Action callback)
    {
        this.callback = callback;
        Play(targetRotation, true);
    }

    private void Play(Vector3 rotation, bool relative)
    {
        transform.DOLocalRotate(targetRotation, 0.6f).SetRelative(relative);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMoveY(0.5f, 0.3f).SetRelative(true));
        mySequence.Append(transform.DOLocalMoveY(-0.5f, 0.3f).SetRelative(true).OnComplete(OnFinish));
    }

    private void OnFinish()
    {
        if (callback != null)
            callback();

        Destroy(this);
    }
}
