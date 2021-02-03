using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleEffect : MonoBehaviour
{
    // User Inputs
    public Transform root;
    [Header("Position")]
    public float amplitude_PosX = 0.5f;
    public float frequency_PosX = 1f;
    public float amplitude_PosY = 0.5f;
    public float frequency_PosY = 1f;
    public float generalOffset_PosX = 0;
    public float generalOffset_PosY = 0;

    [Header("Scale")]
    public float amplitude_ScaleX = 0.5f;
    public float frequency_ScaleX = 1f;
    public float amplitude_ScaleY = 0.5f;
    public float frequency_ScaleY = 1f;


    // Position Storage Variables
    Vector3 oldPos = new Vector3();
    Vector3 tempPos = new Vector3();
    float offsetPosX;
    float offsetPosY;

    Vector3 oldScale = new Vector3();
    Vector3 tempScale = new Vector3();
    float offsetScaleX;
    float offsetScaleY;

    // Use this for initialization
    void Start()
    {
        // Store the starting position & rotation of the object
        oldPos = root.transform.localPosition;
        oldScale = root.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        tempPos = oldPos;
        offsetPosX = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_PosX + generalOffset_PosX) * amplitude_PosX;
        tempPos.x += offsetPosX;
        offsetPosY = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_PosY + generalOffset_PosY) * amplitude_PosY;
        tempPos.y += offsetPosY;

        tempScale = oldScale;
        offsetScaleX = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_ScaleX) * amplitude_ScaleX;
        tempScale.x += offsetScaleX;
        offsetScaleY = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency_ScaleY) * amplitude_ScaleY;
        tempScale.y += offsetScaleY;



        root.transform.localPosition = tempPos;
        root.transform.localScale = tempScale;
    }
}
