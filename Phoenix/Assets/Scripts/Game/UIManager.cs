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

    [Header("GameOver")]
    public GameObject GameOverOverlay;
    

    private PulseAnimation scorePulse;
    private float lifebarWidth, lifebarHeight;
    public float playerEnergy;
    private float targetEnergyPercentage;
    private float currentEnergyPercentage;
    private int score = 0;

    private float normalUpdateSpeed = 0.002f;

    //update
    private float diff = 0;

    void Start()
    {
        //scorePulse = scoreText.GetComponent<PulseAnimation>();
        lifebarWidth = healthbar.rectTransform.sizeDelta.x;
        lifebarHeight = healthbar.rectTransform.sizeDelta.y;
        currentEnergyPercentage = 0;
    }

    void Update()
    {
        //if (MainGameManager.Instance != null && MainGameManager.Instance.gameState != GameState.Main) return;
        if (PrototypeManager.Instance.gameState != GameState.Main) return;

        playerEnergy = PlayerStats.Instance.energy;
        targetEnergyPercentage = playerEnergy / 100.0f;
        diff = targetEnergyPercentage - currentEnergyPercentage;
        currentEnergyPercentage += Mathf.Min(Mathf.Abs(diff), normalUpdateSpeed) * Mathf.Sign(diff);
        healthbar.fillAmount = currentEnergyPercentage;
        //healthbar.rectTransform.sizeDelta = new Vector2(lifebarWidth * energyPercentage, lifebarHeight);

        scoreText.text = "" + PlayerStats.Instance.score;
        return;

        if (score != PlayerStats.Instance.score)
        {
            score = PlayerStats.Instance.score;
            scoreText.text = "" + score;
            //scorePulse.Play();
        }
    }

    public void PushGameEnd()
    {
        GameOver.SetActive(true);
        FinalScore.text = "" + PlayerStats.Instance.score;
    }

    public void UpdateLife(int value)
    {
        //lives.text = (value >= 10) ? "" : "0" + value;
    }

    public void ShowGameOver()
    {
        GameOverOverlay.SetActive(true);
    }

    public void DismissGameOver()
    {
        GameOverOverlay.SetActive(false);
    }
}
