using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using C1.Feedbacks;

public class TitleScreenManager : SingletonBehaviour<TitleScreenManager> 
{
    public C1Feedbacks playButtonFX;
    public C1Feedbacks BGFX;

    private void Start()
    {
        playButtonFX.Play();
        BGFX.Play();
    }
    public void GameStart()
    {
        SceneManager.LoadScene("Playground", LoadSceneMode.Single);
    }
}
