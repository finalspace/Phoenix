using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace C1.Feedbacks
{
    [ExecuteAlways]
    [AddComponentMenu("C1/Feedbacks/C1Feedbacks")]
    [DisallowMultipleComponent]
    public class C1Feedbacks : MonoBehaviour
    {
        public List<C1Feedback> feedbacks = new List<C1Feedback>();

        public bool autoPlay = false;

        protected virtual void Start()
        {
            if (autoPlay)
                Play();
        }

        public virtual void Play()
        {
            for (int i = 0; i < feedbacks.Count; i++)
            {
                feedbacks[i].Play();
            }
        }
    }
}
