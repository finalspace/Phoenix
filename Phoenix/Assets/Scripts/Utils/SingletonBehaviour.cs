using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T s_instance;
    public static T Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<T>();
            }
            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this as T;
        }
    }

    protected virtual void OnDestroy()
    {
        if (s_instance == this)
        {
            s_instance = null;
        }
    }
}