using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public class Food : SimpleTrigger
{
    public C1Feedbacks FB_UseItem;
    public override void Trigger()
    {
        MusicManager.Instance.PlayEat();
        FB_UseItem?.Play();
        //Player.Instance.Kill();
    }
}
