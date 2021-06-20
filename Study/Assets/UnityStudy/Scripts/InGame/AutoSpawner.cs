using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawner : MonoBehaviour
{
    public GameObject spawnOriginal;
    public Transform spawnPos;
    public float spawnFrequency;
    public float spawnTime;
    
    //void Update()
    //{
    //    spawnTime += Time.deltaTime;

    //    if(spawnFrequency < spawnTime)
    //    {
    //        Spawn();

    //        spawnTime = 0;
    //    }
        
    //}

    void Spawn()
    {
        Vector3 offset = new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));
        
        GameObject spawnObj = Instantiate(spawnOriginal);
        if(spawnObj == null)
        {
            return;
        }

        spawnObj.transform.position = spawnPos.position + offset;
    }

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());        
    }

    //IEnumerator : 열거자   
    IEnumerator SpawnCoroutine()
    {
        while(true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnFrequency); // 타임 스케일 영향 받음
        }

        yield return null;
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForSecondsRealtime(spawnFrequency); // 타임 스케일 영향 x 
        //yield return new WaitForFixedUpdate();
        //yield return new WaitUntil(()=> 
        //{ 
        //    return true; }
        //);

        //yield return new WaitWhile(() =>
        //{
        //    return true;
        //});
    }
}
