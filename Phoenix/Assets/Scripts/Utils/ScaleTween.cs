using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    float x, v, xt;
    Vector3 dir;

    // Use this for initialization
    void Start()
    {
        x = 5.0f;
        v = 0.0f;
        xt = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Enlarge();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Shrink();
        }
        SpringUtil.Spring(ref x, ref v, xt, .23f, 8.0f * Mathf.PI, Time.deltaTime);


        //apply animation effect
        DemoEffect();
    }

    void play()
    {
        //xt = 1 - xt;
        //v = 0;
    }

    public void Enlarge()
    {
        xt = 1;
    }

    public void Shrink()
    {
        xt = 5;
    }

    /// <summary>
    /// each line is a different effect
    /// </summary>
    void DemoEffect()
    {
        transform.localScale = Vector3.one * (6 - x);
        //transform.transform.position = new Vector3(0, 0, x);
        //transform.rotation = Quaternion.AngleAxis(x * 90, Vector3.up);
    }

    /*
  x     - value             (input/output)
  v     - velocity          (input/output)
  xt    - target value      (input)
  zeta  - damping ratio     (input)
  omega - angular frequency (input)
  h     - time step         (input)
*/
    void Spring(
         ref float x, ref float v, float xt,
         float zeta, float omega, float h)
    {
        float f = 1.0f + 2.0f * h * zeta * omega;
        float oo = omega * omega;
        float hoo = h * oo;
        float hhoo = h * hoo;
        float detInv = 1.0f / (f + hhoo);
        float detX = f * x + h * v + hhoo * xt;
        float detV = v + hoo * (xt - x);
        x = detX * detInv;
        v = detV * detInv;
    }

    /*
  x     - value             (input/output)
  v     - velocity          (input/output)
  xt    - target value      (input)
  zeta  - damping ratio     (input)
  omega - angular frequency (input)
  h     - time step         (input)
*/
    void SpringSemiImplicitEuler
    (
      ref float x, ref float v, float xt,
      float zeta, float omega, float h
    )
    {
        v += -2.0f * h * zeta * omega * v
             + h * omega * omega * (xt - x);
        x += h * v;
    }
}
