using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugManager : SingletonBehaviour<UIDebugManager>
{
    public Text debugText1;
    public Text DebugText2;
    public Transform Arrow;
    public GameObject wall;

    private bool showWall = false;

    private void Update()
    {
        //DebugText2.text = Player.Instance.playerMovement.GetVelocity().ToString();
    }

    public void ToggleWall()
    {
        showWall = !showWall;
        wall.SetActive(showWall);
    }
}
