using UnityEngine;

public enum CameraShakeState { FadingIn, FadingOut, Sustained, Inactive }

public class CameraShakeInstance
{
    /// <summary>
    /// The intensity of the shake. It is recommended that you use ScaleMagnitude to alter the magnitude of a shake.
    /// </summary>
    public float Magnitude;

    /// <summary>
    /// Roughness of the shake. It is recommended that you use ScaleRoughness to alter the roughness of a shake.
    /// </summary>
    public float Roughness;

    /// <summary>
    /// How much influence this shake has over the local position axes of the camera.
    /// </summary>
    public Vector3 PositionInfluence = Vector3.one;

    /// <summary>
    /// How much influence this shake has over the local rotation axes of the camera.
    /// </summary>
    public Vector3 RotationInfluence = Vector3.one;

    /// <summary>
    /// Should this shake be removed from the CameraShakeInstance list when not active?
    /// </summary>
    public bool DeleteOnInactive = true;

    private Camera camRef;

    CameraShakeState state = CameraShakeState.Inactive;
    public CameraShakeState CurrentState
    {
        get { return state; }
    }

    float camSizeOld = 0;
    float roughMod = 1, magnMod = 1;
    float fadeOutDuration, fadeInDuration, sustainDuration;
    float percentage = 0;
    float progress = 0;
    float tick = 0;
    Vector3 amt;

    /// <summary>
    /// Will create a new instance that will shake once and fade over the given number of seconds.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="sustainTime">How long, in seconds, to play shake at the max amount.</param>
    /// <param name="fadeInTime">How long, in seconds, to fade in the shake.</param>
    /// <param name="fadeOutTime">How long, in seconds, to fade out the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    public CameraShakeInstance(float magnitude, float roughness, float sustainTime, float fadeInTime, float fadeOutTime, Camera cam = null)
    {
        this.Magnitude = magnitude;
        this.Roughness = roughness;
        this.camRef = cam;
        if (cam != null)
            camSizeOld = cam.orthographicSize;
        sustainDuration = sustainTime;
        fadeOutDuration = fadeOutTime;
        fadeInDuration = fadeInTime;
        
        progress = 0;

        if (fadeInTime > 0)
        {
            state = CameraShakeState.FadingIn;
            percentage = 0;
        }
        else
        {
            state = CameraShakeState.Sustained;
            percentage = 1;
        }

        tick = Random.Range(-100, 100);
    }

    /// <summary>
    /// Will create a new instance that will start a sustained shake.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake.</param>
    /// <param name="roughness">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    public CameraShakeInstance(float magnitude, float roughness)
    {
        this.Magnitude = magnitude;
        this.Roughness = roughness;
        state = CameraShakeState.Sustained;

        tick = Random.Range(-100, 100);
    }

    public Vector3 UpdateShake()
    {
        amt.x = Mathf.PerlinNoise(tick, 0) - 0.5f;
        amt.y = Mathf.PerlinNoise(0, tick) - 0.5f;
        amt.z = Mathf.PerlinNoise(tick, tick) - 0.5f;

        state = CameraShakeState.Sustained;

        if (state == CameraShakeState.FadingIn)
        {
            if (fadeInDuration <= 0)
            {
                state = CameraShakeState.Sustained;
                return Vector3.zero;
            }

            if (percentage < 1)
                percentage += Time.deltaTime / fadeInDuration;
            else
            {
                percentage = 1;
                state = CameraShakeState.Sustained;
            }
        }

        if (state == CameraShakeState.Sustained)
        {
            if (sustainDuration < 0)
            {
                state = CameraShakeState.FadingOut;
                return Vector3.zero;
            }
            if (progress < 1)
                progress += Time.deltaTime / sustainDuration;
            else
            {
                if (fadeOutDuration > 0)
                    state = CameraShakeState.FadingOut;
                else
                    state = CameraShakeState.Inactive;
            }
        }

        if (state == CameraShakeState.FadingOut)
        {
            if (sustainDuration < 0)
            {
                state = CameraShakeState.Inactive;
                return Vector3.zero;
            }

            if (percentage > 0)
                percentage -= Time.deltaTime / fadeOutDuration;
            else
                state = CameraShakeState.Inactive;
        }
          


        if (state == CameraShakeState.FadingIn || state == CameraShakeState.FadingOut)
            tick += Time.deltaTime * Roughness * roughMod * percentage;
        else
            tick += Time.deltaTime * Roughness * roughMod;

        if (camRef != null)
            camRef.orthographicSize = camSizeOld - percentage * 0.2f;

        return amt * Magnitude * magnMod * percentage;
    }

    /// <summary>
    /// Scales this shake's roughness while preserving the initial Roughness.
    /// </summary>
    public float ScaleRoughness
    {
        get { return roughMod; }
        set { roughMod = value; }
    }

    /// <summary>
    /// Scales this shake's magnitude while preserving the initial Magnitude.
    /// </summary>
    public float ScaleMagnitude
    {
        get { return magnMod; }
        set { magnMod = value; }
    }

  
}