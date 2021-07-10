using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NaviCharacter : MonoBehaviour
{
    public NavMeshAgent agent;

    private void Update()
    {
        if( Input.GetMouseButtonDown(0))
        {            
            Debug.Log($"X : {Input.mousePosition.x} Y : {Input.mousePosition.y}");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit rayhitInfo))
            {
                agent.SetDestination(rayhitInfo.point);
            }
        }
    }
}
