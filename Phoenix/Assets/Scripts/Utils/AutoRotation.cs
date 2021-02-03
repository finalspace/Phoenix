using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    public float spinSpeedMin;
    public float spinSpeedMax;
    public float minAbsSpeed;

    private float speed;

    private void Start()
    {
        speed = Random.Range(spinSpeedMin, spinSpeedMax);
        int k = 100;
        while (Mathf.Abs(speed) < minAbsSpeed && k-- > 0)
        {
            speed = Random.Range(spinSpeedMin, spinSpeedMax);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }

    public void SetSpeed(float value)
    {
		spinSpeedMin = spinSpeedMax = speed = value;
    }
}
