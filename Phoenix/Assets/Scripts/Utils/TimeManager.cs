using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonBehaviour<TimeManager>
{
    private float slowdownFactor = 0.05f;
    private float slowdownLength = 0.5f;

    private bool slowMode = false;
    private bool pause = false;

    // Update is called once per frame
    void Update()
    {
        if (slowMode)
            return;

        //Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }

    public void SlowMotion()
    {
        slowMode = true;

        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void Reset()
    {
        slowMode = false;
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void TogglePauseGame()
    {
        pause = !pause;
        if (pause)
            Time.timeScale = 0;
        else Time.timeScale = 1;
    }
}
