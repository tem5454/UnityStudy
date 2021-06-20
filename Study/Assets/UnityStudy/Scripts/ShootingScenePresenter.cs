using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScenePresenter : MonoBehaviour, IScenePresenterBase
{
    private void Start()
    {
        SceneStart();
    }

    public void SceneStart()
    {
        //GameObject objectMgr = Resources.Load<GameObject>("ManagerPrefab/ObjectManager");
        GameObject effectMgr = Resources.Load<GameObject>("ManagerPrefab/EffectManager");
        GameObject gameMgr = Resources.Load<GameObject>("ManagerPrefab/GameManager");

        //Instantiate(objectMgr);
        Instantiate(effectMgr);
        Instantiate(gameMgr);
    }

    public void SceneEnd()
    {
    }
}
