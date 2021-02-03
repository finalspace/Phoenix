using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : SingletonBehaviour<CollisionManager>
{
    public LayerMask OneSideGound;
    public LayerMask HardBlock;
    public LayerMask Interactable;
    public LayerMask Recycle;
}
