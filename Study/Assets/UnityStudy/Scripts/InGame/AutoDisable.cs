using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float delaytime;
   
    private void OnEnable()
    {
        Invoke("InvokeAutoDisable", delaytime);
    }

    void InvokeAutoDisable()
    {
        gameObject.SetActive(false);
    }
}
