using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TimePattern
{
    public float cooldown;
}

public class Gun_Pattern : MonoBehaviour
{
    public Transform fireRoot;
    public GameObject bulletPrefab;
    public float angleFrom = 0;
    public float angleTo = 0;
    public float speedFrom = 1.0f;
    public float speedTo = 2.0f;
    public float coolDownMin = 1.0f;
    public float coolDownMax = 3.0f;

    [Header("Bullet")]


    [Tooltip("Initial Cooldown Overwrite(when value is positive).")]
    public float startCooldown = -1;

    [Header("Cooldown Pattern")]
    public List<TimePattern> CDPattern;
    private int patternIdx;

    private Bullet bullet;
    private float cooldown;
    private long lastTime;

    [Header("Fire Warning")]
    public bool fireWarning = false;
    public float warningTime;
    private float prepareTime;
    private bool loaded = false;

    private void Start()
    {
        if(fireRoot == null)
            fireRoot = transform;
        ResetCoolDown(startCooldown);
    }

    private void Update()
    {
        if (fireWarning)
        {
            if (!loaded && DateTimeUtil.MillisecondsElapseFromMilliseconds(lastTime) > prepareTime)
            {
                LoadBullet();
            }
        }

        if (DateTimeUtil.MillisecondsElapseFromMilliseconds(lastTime) > cooldown)
        {
            Fire();
            ResetCoolDown();
        }
    }

    private void LoadBullet()
    {
        float angle = Random.Range(angleFrom, angleTo) + transform.eulerAngles.y - transform.eulerAngles.z;
        float speed = Random.Range(speedFrom, speedTo);
        Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
        bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
		bullet.transform.SetParent(transform);
		bullet.transform.position = fireRoot.position;
        bullet.Init(direction * speed, Player.Instance?.transform);
        bullet.enabled = false;

        loaded = true;
    }

    public void Fire()
    {
        if (!loaded)
            LoadBullet();

		if (bullet)
		{
			bullet.transform.SetParent(null);
			bullet.enabled = true;
		}

        loaded = false;
    }

    private void ResetCoolDown(float cd = -1)
    {
        if (cd > 0)
            cooldown = cd * 1000;
        else
        {
            if (patternIdx == 0)
                cooldown = Random.Range(coolDownMin, coolDownMax) * 1000;
            else
            {
                cooldown = CDPattern[patternIdx - 1].cooldown * 1000;
                if (patternIdx == CDPattern.Count - 1)
                    patternIdx = -1;
            }
            patternIdx++;
        }
        if (fireWarning)
            prepareTime = cooldown - warningTime * 1000;

        lastTime = DateTimeUtil.GetUnixTimeMilliseconds();
    }

}
