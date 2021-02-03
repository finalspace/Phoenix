using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : SingletonBehaviour<UIManager>
{
    public Text scoreText;
    public Text lives;
    public Image healthbar;
    public GameObject GameOver;
    public TMPro.TextMeshProUGUI FinalScore;
    

    private PulseAnimation scorePulse;
    private float lifebarWidth, lifebarHeight;
    private float playerEnergy;
    private float energyPercentage;
    private int score = 0;

    void Start()
    {
        scorePulse = scoreText.GetComponent<PulseAnimation>();
        lifebarWidth = healthbar.rectTransform.sizeDelta.x;
        lifebarHeight = healthbar.rectTransform.sizeDelta.y;
    }

    void Update()
    {
        if (MainGameManager.Instance != null && MainGameManager.Instance.gameState != GameState.Main) return;

        playerEnergy = PlayerStats.Instance.energy;
        energyPercentage = playerEnergy / 100.0f;
        healthbar.rectTransform.sizeDelta = new Vector2(lifebarWidth * energyPercentage, lifebarHeight);

        if (score != PlayerStats.Instance.score)
        {
            score = PlayerStats.Instance.score;
            scoreText.text = "" + score;
            scorePulse.Play();
        }
    }

    public void PushGameEnd()
    {
        GameOver.SetActive(true);
        FinalScore.text = "" + PlayerStats.Instance.score;
    }

    public void UpdateLife(int value)
    {
        lives.text = (value >= 10) ? "" : "0" + value;
    }
}
