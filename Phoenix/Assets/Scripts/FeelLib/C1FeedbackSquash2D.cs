using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C1.Feedbacks
{
    [C1FeedbackAttributeName("Squash2D")]
    public class C1FeedbackSquash2D : C1Feedback
    {
        public Transform squashRoot;
        public enum Timescales { Regular, Unscaled }

        /// whether we should use deltaTime or unscaledDeltaTime;
        public Timescales Timescale = Timescales.Regular;
        public bool playing = false;

        [Header("SingleInstance")]
        public SquashAndStretch2DGlobalTag squashSingleInstance;

        [Header("Rescale")]
        /// the minimum scale to apply to this object
        public Vector2 MinimumScale = new Vector2(0.5f, 0.5f);
        /// the maximum scale to apply to this object
        public Vector2 MaximumScale = new Vector2(2f, 2f);

        [Header("Stretch")]
        public AnimationCurve StretchCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));
        private float stretchVal;
        private float stretchTargetVal = 0;
        private float stretchVel;


        [Header("Squash")]
        /// the curve to apply when squashing the object (this describes scale on x and z, will be inverted for y to maintain mass)
        public AnimationCurve SquashCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1f), new Keyframe(1, 0f));

        public float TimescaleTime { get { return (Timescale == Timescales.Regular) ? Time.time : Time.unscaledTime; } }
        public float TimescaleDeltaTime { get { return (Timescale == Timescales.Regular) ? Time.deltaTime : Time.unscaledDeltaTime; } }

        private Vector3 _newLocalScale;
        private Vector3 _initialScale;

        private float squashStartAt = 0f;
        private float releaseStartAt = 0f;
        private bool _squashing = false;
        private float _squashIntensity;
        private float _squashDuration;

        private bool _movementStarted = false;
        private float _lastVelocity = 0f;

        public float duration;
        public float intensity;

        public float zeta = 0.3f;
        public float omega = 4.0f;

        //update
        private float elapsed;

#if UNITY_EDITOR
        public override Color FeedbackColor { get { return new Color32(255, 212, 0, 255); } }
#endif

        private void Awake()
        {
            //_initialScale = squashRoot.transform.localScale;
            _initialScale = Vector3.one;
        }

        public override void Play()
        {
            if (!Active)
                return;

            base.Play();

            TriggerSquash(duration, intensity);
        }

        protected virtual void LateUpdate()
        {
            ComputeNewLocalScale();
        }


        /// <summary>
        /// Computes a new local scale for this object
        /// </summary>
        protected virtual void ComputeNewLocalScale()
        {
            if (!playing)
                return;

            if (_squashing)
            {
                elapsed = C1Maths.Remap(TimescaleTime - squashStartAt, 0f, _squashDuration, 0f, 1f);
                stretchVal = SquashCurve.Evaluate(elapsed);

                if (elapsed >= 0.5f)
                {
                    _squashing = false;
                    releaseStartAt = TimescaleTime;
                }
            }
            else
            {
                SpringUtil.Spring(ref stretchVal, ref stretchVel, stretchTargetVal, zeta, omega * Mathf.PI, Time.deltaTime);
                if (TimescaleTime - releaseStartAt > _squashDuration * 3)
                {
                    Stop();
                }
            }

            _newLocalScale.x = _initialScale.x + stretchVal * _squashIntensity;
            _newLocalScale.y = _initialScale.y - stretchVal * _squashIntensity * 2;
            _newLocalScale.z = _initialScale.z + stretchVal * _squashIntensity;

            _newLocalScale.x = Mathf.Clamp(_newLocalScale.x, MinimumScale.x, MaximumScale.x);
            _newLocalScale.z = Mathf.Clamp(_newLocalScale.z, MinimumScale.x, MaximumScale.x);
            _newLocalScale.y = Mathf.Clamp(_newLocalScale.y, MinimumScale.y, MaximumScale.y);

            squashRoot.transform.localScale = _newLocalScale;
        }

        //event
        public void TriggerSquash(float duration, float intensity)
        {
            //single instance control
            if (squashSingleInstance != null)
            {
                if (squashSingleInstance.activeSquash != null)
                {
                    squashSingleInstance.activeSquash.Stop();
                }
                squashSingleInstance.activeSquash = this;
            }

            squashStartAt = TimescaleTime;
            _squashing = true;
            _squashIntensity = intensity;
            _squashDuration = duration;
            playing = true;
        }

        public void Stop()
        {
            playing = false;
            if (squashSingleInstance != null)
            {
                squashSingleInstance.activeSquash = null;
            }
        }
    }
}
