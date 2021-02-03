using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    public Transform pulseRoot;
    public bool loop = false;

    public bool autoStart = false;
    [Header("Basic")]
    public float pulseScale = 1.5f;
    public float phaseTime = 0.5f;

    [Header("Style")]
    public Ease easeType = Ease.OutCubic;
    public LoopType loopType = LoopType.Restart;

    private string id;
    private float scale;
    private bool playing = false;

    private void Start()
    {
        id = "pulse" + System.Guid.NewGuid();

        if (pulseRoot == null)
            pulseRoot = transform;

        if (autoStart)
            Play(loop);
    }

    private void Update()
    {
        if (!playing)
            scale = 1;

        pulseRoot.transform.localScale = Vector3.one * scale;
    }

    public void Play(bool loop = false)
    {
        DOTween.Kill(id);
        scale = pulseScale;
        playing = true;
        int loopTimes = loop ? -1 : 1;
        DOTween.To(() => scale, x => scale = x, 1, phaseTime).SetLoops(loopTimes, loopType).SetEase(easeType).SetId(id).OnComplete(OnComplete);
    }

    private void OnComplete()
    {
        playing = false;
    }
}