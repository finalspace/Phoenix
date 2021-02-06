using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C1.Feedbacks
{
    [C1FeedbackAttributeName("Cam Shake")]
    public class C1FeedbackCamShake : C1Feedback
    {
        public Camera cam;
        public float magnitude = 1.0f;
        public float roughness = 8;
        public float zoomPower = 0.2f;
        public float sustainTime = 1;
        public float fadeInTime = 0;
        public float fadeOutTime = 0.5f;

#if UNITY_EDITOR
        public override Color FeedbackColor { get { return new Color32(255, 212, 0, 255); } }
#endif

        public override void Play()
        {
            if (!Active)
                return;

            base.Play();

            CameraShakeInstance shakeInstance = new CameraShakeInstance(magnitude, roughness, sustainTime, fadeInTime, fadeOutTime, zoomPower, cam);
            CameraShaker camShaker = cam.gameObject.AddComponent<CameraShaker>();
            camShaker.Init(shakeInstance);
            camShaker.Play();
        }
    }
}
