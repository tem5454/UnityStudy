using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletTargetType
{
    None = 0,
    ToPlayer = 1,
    ToEnemy = 2,
}

public class BulletBase : MonoBehaviour
{
    public float speed = 0.0f;
    public float range = 0;
    public float damage = 0.0f;
    private float lifeTime = 5.0f;
    public Vector3 startPos;
    public BulletTargetType targetType;

    private void OnEnable()
    {
        Invoke(nameof(AutoDisable), lifeTime);
    }

    void AutoDisable()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {    
        float distance = Vector3.Distance(startPos, transform.position);
        if( range < distance)
        {
            this.gameObject.SetActive(false);
        }

        //transform.localPosition += new Vector3(0, 0, speed * Time.deltaTime);
        //Translate 자기 자신의 좌표를 움직이는

        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    public void Init(Vector3 startPos, Quaternion startRot, float range, float damage, float speed, BulletTargetType type)
    {
        this.startPos = startPos;
        this.range = range;
        this.damage = damage;
        this.speed = speed;
        this.transform.rotation = startRot;
        this.transform.position = startPos;
        this.targetType = type;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.CompareTag("Prop"))
        {
            GameObject obj = EffectManager.Instance.GetEffect(EffectType.BulletA_Impact);
            obj.transform.position = collision.contacts[0].point;
            obj.SetActive(true);

            gameObject.SetActive(false);
        }
        else if(LayerMask.NameToLayer("Character") == collision.gameObject.layer)
        {
            if (collision.gameObject.CompareTag("Enemy") && targetType == BulletTargetType.ToEnemy)
            {
                GameObject obj = EffectManager.Instance.GetEffect(EffectType.BulletA_Impact);
                obj.transform.position = collision.contacts[0].point;
                obj.SetActive(true);

                gameObject.SetActive(false);

                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
                if (enemy)
                {
                    enemy.Damage(10);
                }
            }
            else if(collision.gameObject.CompareTag("Player") && targetType == BulletTargetType.ToPlayer)
            {
                GameObject obj = EffectManager.Instance.GetEffect(EffectType.BulletB_Impact);
                obj.transform.position = collision.contacts[0].point;
                obj.SetActive(true);

                gameObject.SetActive(false);

                TankController.Player.Damage(10);
            }
        }
    }
}
