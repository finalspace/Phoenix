using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    // User Inputs
    public Transform root;
    public float amplitude_updown = 0.5f;
    public float frequency_updown = 1f;
    public float amplitude_leftright = 0.5f;
    public float frequency_leftright = 1f;
    public float amplitude_rotate = 0.5f;
    public float frequency_rotate = 1f;
    public float rotateOffset = 1;
    public float generalOffset = 0;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();
    Vector3 baseRotate = new Vector3();
    Vector3 tempRotate = new Vector3();
    float offsetUpdown;
    float offsetLeftright;
    float ratio = 0.2f;

    private bool floating = true;

    // Use this for initialization
    void Start()
    {
        // Store the starting position & rotation of the object
        if (root == null)
            root = transform;

        posOffset = root.transform.localPosition;
        baseRotate = root.transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin object around Y-Axis
        //transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
        if (!floating)
            return;

        tempPos = posOffset;
        offsetUpdown = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_updown + generalOffset) * amplitude_updown;
        tempPos.y += offsetUpdown;
        //tempPos.x += offsetHeight * ratio;

        offsetLeftright = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_leftright + generalOffset) * amplitude_leftright;
        tempPos.x += offsetLeftright;


        tempRotate = baseRotate + new Vector3(0, 0, Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_rotate + generalOffset + rotateOffset) * amplitude_rotate);

        root.transform.localPosition = tempPos;
        root.transform.localRotation = Quaternion.Euler(tempRotate.x, tempRotate.y, tempRotate.z);
    }

    public void PauseAnimation()
    {
        floating = false;
    }

    public void PlayAnimation()
    {
        floating = true;
    }
}
