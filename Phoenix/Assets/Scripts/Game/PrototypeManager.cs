using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeManager : SingletonBehaviour<PrototypeManager>
{
    public GameObject BG;
    public GameState gameState;
    // Update is called once per frame

    public void Start()
    {
        gameState = GameState.Main;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BG.SetActive(!BG.activeSelf);
        }
    }

    public void TogglePause()
    {
        TimeManager.Instance.TogglePauseGame();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        gameState = GameState.Lose;
        UIManager.Instance.ShowGameOver();
    }
}
