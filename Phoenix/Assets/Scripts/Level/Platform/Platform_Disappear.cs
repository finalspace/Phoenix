using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public class Platform_Disappear : PlatformBase
{
    [Header("Platform_Disappear")]
    public C1Feedbacks FB_Countdown;
    public C1Feedbacks FB_Disappear;
    public C1FeedbackWobble2D wobble2D;

    public float countDownTime = 2.0f;

    public GameObject sprite;
    public BoxCollider2D colliderBox;

    private bool countDownStarted = false;
    private float countDownAt;

    //update
    private float timeLeft;

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ObjLanding(null);
        }

        if (!countDownStarted)
            return;

        timeLeft = countDownTime - (Time.time - countDownAt);
        if (timeLeft < 0.5f && wobble2D)
        {
            wobble2D.duration = 0.2f;
        }
        if (timeLeft < 0)
        {
            Disappear();
        }
    }

    public override void ObjLanding(GameObject obj)
    {
        FB_Countdown?.Play();
        countDownAt = Time.time;
        countDownStarted = true;
    }

    public void Disappear()
    {
        countDownStarted = false;

        sprite.SetActive(false);
        colliderBox.enabled = false;
        FB_Disappear?.Play();

        Invoke("TrueDestroy", 0.2f);
    }

    public void TrueDestroy()
    {
        //todo: kill character
        if (Player.Instance != null && Player.Instance.platform == this)
        {
            Player.Instance.Kill();
        }
        Recycle();
    }
}
