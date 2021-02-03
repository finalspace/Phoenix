using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonBehaviour<LevelManager>
{
    private int level = 3;
    //private string sceneName = "mainlevel art";

    // Use this for initialization 
    void Awake()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        InitGame();
    }

    void InitGame()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        //SceneManager.UnloadSceneAsync(sceneName);
        //MainGameManager.Instance.GoToTitleScene();
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
