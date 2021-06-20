using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectType
{
    None = 0,

    BulletA_Impact,
    BulletA_Muzzle,
    BulletA_Projectile,

    BulletB_Impact,
    BulletB_Muzzle,
    BulletB_Projectile,

    BulletC_Impact,
    BulletC_Muzzle,
    BulletC_Projectile,

    Explosion_A,

    End,
}

[System.Serializable]
public class EffectBase
{
    public string effectName;
    public GameObject original;
    public Stack<GameObject> effectPool = new Stack<GameObject>();
    public int poolSize = 0;
}

public class EffectManager : SingletonBase<EffectManager>
{
    [System.Serializable]
    public class DictionaryEffectTypeAndBase : SerializableDictionary<EffectType, EffectBase>
    {

    }

    public DictionaryEffectTypeAndBase effectContainer = new DictionaryEffectTypeAndBase();

    public void Awake()
    {
        foreach(var effect in effectContainer)
        {
            for(int i = 0; i < effect.Value.poolSize; ++i)
            {
                GameObject obj = Instantiate(effect.Value.original);
                obj.SetActive(false);
                effect.Value.effectPool.Push(obj);
            }            
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject GetEffect(EffectType _effectType)
    {
        GameObject obj = effectContainer[_effectType].effectPool.Pop();
        if(obj.activeSelf)
        {
            effectContainer[_effectType].effectPool.Push(obj);

            GameObject newObj = Instantiate(effectContainer[_effectType].original);
            obj = newObj;
            obj.SetActive(false);
            effectContainer[_effectType].effectPool.Push(newObj);
        }
        else
        {
            effectContainer[_effectType].effectPool.Push(obj);
        }        

        return obj;
    }
}
