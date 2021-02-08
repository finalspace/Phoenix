using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleByDistance : MonoBehaviour
{
    public Transform refRoot;
    public float distance;

    private void Awake()
    {
        if (refRoot == null)
        {
            refRoot = CameraManager.Instance.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(refRoot.position, transform.position) > distance)
        {
            Destroy(gameObject);
        }
    }
}
