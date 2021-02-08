using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using C1.Feedbacks;

public class TitleScreenManager : SingletonBehaviour<TitleScreenManager> 
{
    public C1Feedbacks playButtonFX;

    private void Start()
    {
        playButtonFX.Play();
    }
    public void GameStart()
    {
        SceneManager.LoadScene("Playground", LoadSceneMode.Single);
    }
}
