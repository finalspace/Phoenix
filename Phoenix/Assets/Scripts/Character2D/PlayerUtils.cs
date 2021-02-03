using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtils {

    public delegate void PlayerDead();
    public static event PlayerDead OnPlayerDead;


    public static void PlayerDeadOccur()
    {
        OnPlayerDead();
    }
}
