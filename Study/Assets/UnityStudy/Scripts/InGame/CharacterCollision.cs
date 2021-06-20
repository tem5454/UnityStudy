using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    public GameObject master;
    public GameObject spawnobj;

    // 레이어, 태그
    // 콜리젼에선 레이어를 먼저 사용

    //Collider, Collision 차이
    //Collision은 부딫친 면의 위치 , 각도, 힘 같은 정보들
    //Collider 부딫친 상대방 콜라이더 객체

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Debug.Log("Layer : " + collision.gameObject.layer);
    //    Debug.Log("Enter :" + collision.gameObject.name);
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log("Stay :" + collision.gameObject.name);
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    Debug.Log("Exit :" + collision.gameObject.name);
    //}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter :" + other.gameObject.name);

        if(other.CompareTag("EventZone"))
        {
            EventZoneBase eventZoneBase = other.GetComponent<EventZoneBase>();
            if (eventZoneBase == null)
            {
                return;
            }

            if (eventZoneBase.eventZoneType != EventZoneBase.EventZoneType.Spawner)
            {
                eventZoneBase.OnEventZone(master);
            }            

            Debug.Log("Trigger OnEventZone :" + other.gameObject.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay :" + other.gameObject.name);        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit :" + other.gameObject.name);

        EventZoneBase eventZoneBase = other.GetComponent<EventZoneBase>();
        if (eventZoneBase == null)
        {
            return;
        }

        if (eventZoneBase.eventZoneType == EventZoneBase.EventZoneType.Spawner)
        {
            eventZoneBase.OnEventZone(spawnobj);

            Debug.Log("Trigger OnEventZone Exit :" + other.gameObject.name);
        }
    }
}
