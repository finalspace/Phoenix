using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public delegate void DelePlayerLand(Vector3 vel);
    public static event DelePlayerLand OnPlayerLand;

    public delegate void DelePlayerJump(Vector3 vel);
    public static event DelePlayerJump OnPlayerJump;

    public delegate void DelePlayerJumpFail();
    public static event DelePlayerJumpFail OnPlayerJumpFail;

    public static void PlayerLand(Vector3 vel)
    {
        OnPlayerLand(vel);
    }

    public static void PlayerJump(Vector3 vel)
    {
        //OnPlayerJump(vel);
    }

    public static void PlayerJumpFail()
    {
        //OnPlayerJumpFail();
    }
}
