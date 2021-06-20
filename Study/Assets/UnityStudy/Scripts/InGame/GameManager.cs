using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    public float PlayTime { get { return this.playTime; } }
    public int PlayScore {  get { return this.playScore; } }

    public float spawnSequence;
    public bool IsPause;
    public float playTime;
    public int playScore;

    private void OnEnable()
    {
        StartCoroutine(Cospawn());
    }

    private void Start()
    {
        
    }
    IEnumerator Cospawn()
    {
        yield return new WaitForEndOfFrame();

        while(true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnSequence);
        }
    }
    public void SpawnEnemy()
    {
        GameObject obj = ObjectManager.Instance.GetObject(ObjectType.Enemy_A);
        if(obj != null)
        {
            obj.transform.position = new Vector3(Random.Range(-33.0f, 33.0f), 0, 25);
            EnemyAI enemyAI = obj.GetComponent<EnemyAI>();
            if (enemyAI)
            {
                enemyAI.Initialized(100, (EnemyAI.AIPattern)Random.Range((int)EnemyAI.AIPattern.Horizontal, (int)EnemyAI.AIPattern.End));
            }
            obj.SetActive(true);
        }        
    }

    public void AddScore(int score )
    {
        playScore += score;
        ShootingUI shootingUI = UIManager.Instance.GetUI<ShootingUI>(UIList.ShootingUI);
        if(shootingUI)
        {
            shootingUI.RefreshScore();
        }
    }

    private void Update()
    {
        if( false == IsPause )
        {
            playTime += Time.unscaledDeltaTime;
        }
    }
}
