using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Dash : SimpleTrigger
{
	public override void Trigger()
    {
		Player.Instance.GrantDash();
    }
}
