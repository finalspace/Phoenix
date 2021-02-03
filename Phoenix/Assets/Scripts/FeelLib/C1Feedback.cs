using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C1.Feedbacks
{
    [System.Serializable]
    public abstract class C1Feedback : MonoBehaviour
    {
        public bool Active = true;
        public GameObject Owner { get; set; }
        public string Label = "C1Feedback";

#if UNITY_EDITOR
        public virtual Color FeedbackColor { get { return Color.white; } }
#endif

        public Transform root;

        private void Awake()
        {
            if (root == null)
                root = transform;
        }

        public virtual void Play()
        {
            if (!Active)
                return;
        }
    }
}
