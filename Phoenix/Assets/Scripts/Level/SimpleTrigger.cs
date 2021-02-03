using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTrigger : MonoBehaviour
{
    public UnityEvent triggerEvent;

    public bool active = true;
    public bool destroyAfterHit = false;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        if (other.tag != "Player")
            return;

        triggerEvent.Invoke();
        Trigger();
        PostTrigger();
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!active) return;

        if (other.tag != "Player")
            return;

        triggerEvent.Invoke();
        Trigger();
        PostTrigger();
    }

    public virtual void Trigger()
    {
        //does nothing
    }

    public virtual void PostTrigger()
    {
        if (destroyAfterHit)
        {
            Destroy(gameObject);
        }
    }
}
