using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C1.Feedbacks;

public abstract class PlatformBase : MonoBehaviour
{
    [Header("Level")]
    public PlatformBase leftParent;
    public PlatformBase rightParent;
    public PlatformBase leftChild;
    public PlatformBase rightChild;
    public Transform nextRoot;

    [Header("Effect")]
    public C1Feedbacks FB_Landing;
    public C1Feedbacks FB_Releasing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
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

    public virtual void ObjLanding(GameObject obj)
    {
        FB_Landing?.Play();
    }

    public virtual void ObjLeaving()
    {
        FB_Releasing?.Play();
    }

    public virtual void Recycle()
    {
        Destroy(gameObject);
    }
}
