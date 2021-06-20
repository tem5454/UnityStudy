using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    None = 0,

    Enemy_A,

    End,
}

[System.Serializable]
public class ObjectBase
{
    public string ObjectName;
    public GameObject original;
    public Stack<GameObject> ObjectPool = new Stack<GameObject>();
    public int poolSize = 0;
}

public class ObjectManager : SingletonBase<ObjectManager>
{
    [System.Serializable]
    public class DictionaryObjectTypeAndBase : SerializableDictionary<ObjectType, ObjectBase>
    {

    }

    public DictionaryObjectTypeAndBase ObjectContainer = new DictionaryObjectTypeAndBase();

    public void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    public void Awake()
    {
        foreach (var Object in ObjectContainer)
        {
            for (int i = 0; i < Object.Value.poolSize; ++i)
            {
                GameObject obj = Instantiate(Object.Value.original);
                obj.SetActive(false);
                Object.Value.ObjectPool.Push(obj);
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject GetObject(ObjectType _ObjectType)
    {
        GameObject obj = ObjectContainer[_ObjectType].ObjectPool.Pop();
        if (obj.activeSelf)
        {
            ObjectContainer[_ObjectType].ObjectPool.Push(obj);

            GameObject newObj = Instantiate(ObjectContainer[_ObjectType].original);
            obj = newObj;
            obj.SetActive(false);
            ObjectContainer[_ObjectType].ObjectPool.Push(newObj);
        }
        else
        {
            ObjectContainer[_ObjectType].ObjectPool.Push(obj);
        }

        return obj;
    }
}
