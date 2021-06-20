using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventZoneBase : MonoBehaviour
{
    public enum EventZoneType
    {
        None,
        ZeroPortal,
        LobbyScenePortal,
        Spawner,
    }

    public enum SceneType
    {
        GameScene,
        LobbyScene,
    }

    public EventZoneType eventZoneType = EventZoneType.None;

    public void OnEventZone(GameObject obj)
    {
        switch (eventZoneType)
        {
            case EventZoneType.ZeroPortal:
                {
                    obj.transform.position = Vector3.zero;
                }
                break;
            case EventZoneType.LobbyScenePortal:
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(SceneType.LobbyScene.ToString());
                }
                break;
            case EventZoneType.Spawner:
                {
                    Vector3 randomPos = new Vector3();
                    randomPos.x = Random.Range(0, 100);                  
                    randomPos.z = Random.Range(0, 100);
                    randomPos.y = 1;

                    GameObject copyObj = Instantiate(obj);
                    copyObj.transform.position = randomPos;
                }
                break;
            default:
                break;
        }
    }
}
