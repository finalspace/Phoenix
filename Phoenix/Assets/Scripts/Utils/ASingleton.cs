using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class ASingleton<TClass> where TClass : class, new()
{
    private static TClass s_Instance;

    /// <summary>
    /// Intentionally different per specific-class.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static bool s_IsConstructing;

    public override string ToString()
    {
        var builder = new StringBuilder("ASingleton<").
            Append(typeof(TClass)).
            Append(">(NumCreated=").
            Append(")");

        return builder.ToString();
    }

    /// <summary>
    /// Query the instance of this singleton.
    /// If the singleton instance does not exist then it is created.
    ///
    /// May set to override.
    /// </summary>
    ///
    /// <returns>
    /// If the singleton is currently being created, else error and returns null.
    /// </returns>
    public static TClass Instance
    {
        get
        {
            return CreateIfNeeded() ? s_Instance : null;
        }
        set
        {
            s_Instance = value;
        }
    }

    /// <summary>
    /// Do any cleanup required for this singleton instance to shut it down.
    /// Also unregisters this instance as THE singleton if it is the globally accessible instance.
    /// 
    /// Note:
    ///     (1) Derived classes should override and do any other cleanup logic they need to do for
    ///         this function like unregistering for events
    ///     (2) Make sure to still call this base class in derived implementations however!
    /// </summary>
    public virtual void Destroy()
    {
        if (object.ReferenceEquals(s_Instance, this))
        {
            s_Instance = null;
        }
    }

    public static bool CreateIfNeeded()
    {
        if (s_Instance == null)
        {
            if (s_IsConstructing)
            {
                Debug.LogError("Constructing");
                return false;
            }
            s_IsConstructing = true;
            s_Instance = new TClass();
            s_IsConstructing = false;
        }
        return true;
    }
}
