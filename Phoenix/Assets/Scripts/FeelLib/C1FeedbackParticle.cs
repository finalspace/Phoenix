using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C1.Feedbacks
{
    [C1FeedbackAttributeName("Spawn Particle")]
    public class C1FeedbackParticle : C1Feedback
    {
        public ParticleSystem particleObj;
        public GameObject particlePrefab;

#if UNITY_EDITOR
        public override Color FeedbackColor { get { return new Color32(0, 242, 255, 255); } }
#endif

        public override void Play()
        {
            if (!Active)
                return;

            base.Play();
            if (particleObj != null)
            {
                particleObj.Play();
            }
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, root.position, Quaternion.identity);
            }
        }
    }
}
