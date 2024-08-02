using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : class, new()
{
    public static T Instance;
    public virtual void Awake()
    {
        Instance = this as T;
    }
}
