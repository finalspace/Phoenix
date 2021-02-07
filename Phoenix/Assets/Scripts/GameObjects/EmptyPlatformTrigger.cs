using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPlatformTrigger : SimpleTrigger
{
    public override void Trigger()
    {
        Player.Instance.Kill();
    }
}
