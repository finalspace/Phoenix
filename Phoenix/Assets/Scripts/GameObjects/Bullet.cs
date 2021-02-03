using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loadable attribute: show sprite but no movement nor damage
/// </summary>
public class Bullet : MonoBehaviour
{
    public Transform visualRoot;
    public Vector3 velocity;

    [Header("Homing")]
    public bool homing = false;
    public Transform target;
    public float power = 0.05f;
    public float trackingTime = -1;

    public SimpleTrigger trigger;
    private float speed;
    protected bool isDefaultMovement = true;

    protected virtual void OnEnable()
    {
		trigger.active = true;
    }

    protected virtual void OnDisable()
    {
		trigger.active = false;
    }

    private void Awake()
	{
		if (trigger == null)
			trigger = GetComponent<SimpleTrigger>();
	}

    private void Start()
    {
        if (trackingTime < 0)
            trackingTime = float.MaxValue;
    }

    public virtual void Init(Vector3 vel)
	{
		velocity = vel;
        speed = velocity.magnitude;
	}

    public virtual void Init(Vector3 vel, Transform t)
    {
        target = t;
        Init(vel);
    }

    private void Update()
    {
        if (!isDefaultMovement) return;

        if (homing && target != null)
        {
            velocity += (target.position - transform.position).normalized * power;
            velocity = velocity.normalized * speed;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            visualRoot.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            trackingTime -= Time.deltaTime;
            if (trackingTime < 0)
                homing = false;
        }
        transform.Translate(velocity * Time.deltaTime);
    }
}
