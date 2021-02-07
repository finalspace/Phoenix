using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrototypeManager : MonoBehaviour
{
    public GameObject BG;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BG.SetActive(!BG.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TimeManager.Instance.TogglePauseGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
