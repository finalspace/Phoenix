using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public class Platform : MonoBehaviour
{
    [Header("Level")]
    public Platform leftParent;
    public Platform rightParent;

    [Header("Effect")]
    public C1Feedbacks FB_Landing;
    public C1Feedbacks FB_Releasing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FB_Landing.Play();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FB_Releasing.Play();
        }
    }

    public void ObjLanding()
    {
        FB_Landing?.Play();
    }

    public void ObjLeaving()
    {
        FB_Releasing?.Play();
    }
}
