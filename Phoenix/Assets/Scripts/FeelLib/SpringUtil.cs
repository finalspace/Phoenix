using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringUtil : MonoBehaviour
{

    public static void Spring(
        ref float x, ref float v, float xt, float h)
    {
        Spring(ref x, ref v, xt, 0.23f, 8.0f * Mathf.PI, h);
    }
/*
    x     - value             (input/output)
    v     - velocity          (input/output)
    xt    - target value      (input)
    zeta  - damping ratio     (input)
    omega - angular frequency (input)
    h     - time step         (input)
*/
    public static void Spring(
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
    public static void SpringSemiImplicitEuler
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
