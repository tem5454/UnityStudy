using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SingletonBase<T> : MonoBehaviour where T : class
{
    public static T Instance { get { return _instance.Value; } }
    private static readonly Lazy<T> _instance = new Lazy<T>(()=> {
        T instance = FindObjectOfType(typeof(T)) as T; 
        if(instance == null)
        {
            GameObject obj = new GameObject(typeof(T).ToString());
            instance = obj.AddComponent(typeof(T)) as T;
            DontDestroyOnLoad(obj);
        }

        return instance;
    }); 
}
