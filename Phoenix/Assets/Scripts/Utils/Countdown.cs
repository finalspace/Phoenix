using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Countdown : MonoBehaviour
{
    public float time;
    public UnityEvent onCompleteEvent = new UnityEvent();
    public UnityAction OnComplete;

    public bool autoStart = false;

    private bool playing = false;

    private void Start()
    {
        if (autoStart)
            StartCoundDown(time);
    }

    private void Update()
    {
        if (!playing) return;

        time -= Time.deltaTime;
        if (time < 0)
        {
            Done();
        }
    }

    public void StartCoundDown(float value, UnityAction onComplete = null)
    {
        onCompleteEvent.AddListener(onComplete);
        OnComplete = onComplete;
        time = value;
        playing = true;
    }

    private void Done()
    {
        playing = false;
        onCompleteEvent.Invoke();
        onCompleteEvent.RemoveListener(OnComplete);

        Destroy(this);
    }
}
