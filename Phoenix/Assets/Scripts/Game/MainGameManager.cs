using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Title, Intro, Main, Win, Lose
}


public class MainGameManager : SingletonBehaviour<MainGameManager> {
    
    public GameState gameState = GameState.Title;

	private void Start()
	{
        if (gameState == GameState.Title)
            GoToTitleScene();
    }

	private void Update()
	{
        if (gameState == GameState.Main)
        {
            HandleInput_MainGame();
        }
        else if (gameState == GameState.Win || gameState == GameState.Lose)
        {
            HandleInput_GameOver();
        }
	}

    public GameState CurrentState()
    {
        return gameState;
    }

    public void GoToTitleScene()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Additive);
        MusicManager.Instance.TitleMusic();
        MusicManager.Instance.PlayBGMusic();
    }

    public void GameStart()
    {
        if (gameState == GameState.Main)
            return;
        
        gameState = GameState.Main;
		SceneManager.LoadScene("MainLevel Art", LoadSceneMode.Additive);
        MusicManager.Instance.GameMusic();
        MusicManager.Instance.PlayBGMusic();
    }

    public void GameLost()
    {
        gameState = GameState.Lose;
        //PlayerUtils.PlayerDeadOccur();
        UIManager.Instance.PushGameEnd();
    }

    private void HandleInput_Title()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }

    private void HandleInput_MainGame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

    private void HandleInput_GameOver()
    {
    }

}
