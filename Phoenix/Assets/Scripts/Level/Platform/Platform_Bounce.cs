using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;
using UnityEngine.Events;

public class Platform_Bounce : PlatformBase
{
    public C1Feedbacks FB_Bounce;

    public int bounceDirection = 0;

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ObjLanding(null);
        }
    }

    public override void ObjLanding(GameObject obj)
    {
        Player player = obj.GetComponent<Player>();
        if (player != null)
        {
            StartCoroutine(delayedAction(() => {
                int dir = bounceDirection == 0 ? player.lastJumpDir : bounceDirection;
                player.Jump(dir);
                FB_Bounce?.Play();
            }, 1));
        }

    }

    IEnumerator delayedAction(UnityAction action, int nFrames)
    {
        yield return null;
        for (int i = 0; i < nFrames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        action.Invoke();
    }
}
