using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : SimpleTrigger
{
	public override void Trigger()
    {
		Player.Instance.Damage();
    }
}
