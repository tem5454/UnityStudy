using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum AIPhase
    {
        None = 0,
        Intro = 1,
        Free = 2,
        Outro = 3,
    }

    public enum AIPattern
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Circle = 3,
        End,
    }

    public float HP;
    public AIPattern Pattern;
    public AIPhase Phase;

    public float FreePhaseTime;

    public float IntroDistance;

    public float IntroSpeed;
    public float FreeSpeed;
    public float OutroSpeed;

    public float FreeMoveValue;

    public Vector3 StartPos;

    public int ShootCount;
    public float ShootDelay;

    public Transform FirePos;
    public Transform Turret;

    public bool IsIntro;

    public Renderer[] ModelRenders;
    public bool IsShining;

    private void Awake()
    {
        IsShining = false;
        ModelRenders = GetComponentsInChildren<Renderer>();
    }


    public void Initialized(float hp, AIPattern pattern)
    {
        HP = hp;
        Pattern = pattern;
        Phase = AIPhase.Intro;
        ShootCount = 0;
        IsIntro = false;
    }

    public void Damage(float damage)
    {
        if(HP - damage < 0)
        {
            gameObject.SetActive(false);
            GameObject effect = EffectManager.Instance.GetEffect(EffectType.Explosion_A);
            if (effect)
            {
                effect.transform.position = transform.position;
                effect.SetActive(true);                
            }

            GameManager.Instance.AddScore(100);
        }
        else
        {
            HP -= damage;

            if(IsShining == false)
            {
                StartCoroutine(CoShineMaterial());
            }            
        }       
    }

    IEnumerator CoShineMaterial()
    {
        IsShining = true;
        int shineCount = 0;

        while(shineCount <10)
        {
            yield return new WaitForSeconds(0.1f);

            Color[] colors = new Color[ModelRenders.Length];

            for (int i = 0; i < ModelRenders.Length; ++i)
            {
                colors[i] = ModelRenders[i].material.color;

                ModelRenders[i].material.color = Color.white;
            }

            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < ModelRenders.Length; ++i)
            {
                ModelRenders[i].material.color = colors[i];
            }

            ++shineCount;
        }

        IsShining = false;
    }

    void Shoot()
    {
        AimToPlayer();

        if (10 < ShootCount)
        {
            return;
        }

        GameObject bullet = EffectManager.Instance.GetEffect(EffectType.BulletB_Projectile);
        if(bullet)
        {
            BulletBase bulletBase = bullet.GetComponent<BulletBase>();
            if (bulletBase == null)
            {
                bulletBase = bullet.AddComponent<BulletBase>();
            }

            bulletBase.Init(FirePos.position, Turret.rotation, 100, 10, 20, BulletTargetType.ToPlayer);

            bullet.SetActive(true);
            ++ShootCount;
            Invoke(nameof(Shoot), ShootDelay);
        }
    }

    private void OnEnable()
    {
        StartPos = transform.position;
    }

    void Update()
    {
        switch(Phase)
        {
            case AIPhase.Intro:
            {
                if (Vector3.Distance(StartPos, transform.position) < IntroDistance)
                {
                    transform.position += Vector3.back * IntroSpeed * Time.deltaTime;
                }
                else
                {
                    InvokePhaseChange();
                }
                break;
            }
            case AIPhase.Free:
            {
                MovePattern();
                AimToPlayer();
                break;
            }
            case AIPhase.Outro:
            {
                transform.position += Vector3.back * OutroSpeed * Time.deltaTime;
                break;
            }
            default:
            {
                break;
            }
        }
    }

    void AimToPlayer()
    {
        Turret.LookAt(new Vector3(TankController.Player.transform.position.x, Turret.transform.position.y, TankController.Player.transform.position.z));
    }

    void MovePattern()
    {
        Vector3 moveVec = new Vector3();

        switch (Pattern)
        {
            case AIPattern.Horizontal:
            {
                moveVec.x = (Mathf.PingPong(Time.time, 10) - 5) * Time.deltaTime;
                break;
            }
            case AIPattern.Vertical:
            {
                moveVec.z = (Mathf.PingPong(Time.time, 10) - 5) * Time.deltaTime;
                break;
            }
            case AIPattern.Circle:
            {
                FreeMoveValue += FreeSpeed * Time.deltaTime;
                moveVec.x = 5 * Mathf.Cos(FreeMoveValue) * Time.deltaTime;
                moveVec.z = 5 * Mathf.Sin(FreeMoveValue) * Time.deltaTime;
                break;
            }
            default:
            {
                break;
            }
        }

        transform.position += moveVec;
    }

    void InvokePhaseChange()
    {
        switch (Phase)
        {
            case AIPhase.Intro:
            {
                Phase = AIPhase.Free;
                Shoot();
                Invoke(nameof(InvokePhaseChange), FreePhaseTime);
                break;
            }
            case AIPhase.Free:
            {
                Phase = AIPhase.Outro;
                InvokePhaseChange();
                break;
            }
            case AIPhase.Outro:
            {
                break;
            }
            default:
            {
                break;
            }
        }
    }
}
