using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public class Food_Egg : SimpleTrigger
{
    public C1Feedbacks FB_UseItem;
    public override void Trigger()
    {
        MusicManager.Instance.PlayDamage();
        FB_UseItem?.Play();
        Player.Instance.EatDamage();
    }
}
