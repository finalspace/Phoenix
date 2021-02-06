using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public class Platform_Disappear : PlatformBase
{
    public C1Feedbacks FB_Countdown;

    public float countDownTime = 2.0f;

    private bool countDownStarted = false;
    private float countDownAt;

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ObjLanding(null);
        }

        if (!countDownStarted)
            return;

        if (Time.time - countDownAt > countDownTime)
        {
            Recycle();
        }
    }

    public override void ObjLanding(GameObject obj)
    {
        FB_Countdown?.Play();
        countDownAt = Time.time;
        countDownStarted = true;
    }

    public override void ObjLeaving()
    {
        base.ObjLeaving();
    }

    public void Disappear()
    {

    }
}
