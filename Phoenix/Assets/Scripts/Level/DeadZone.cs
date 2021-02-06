using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Recycle"))
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Recycle"))
        {
            Destroy(other.gameObject);
        }
    }
}