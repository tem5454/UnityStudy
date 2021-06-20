using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public static TankController Player;
    public float moveSpeed;
    public Rigidbody rigidbody;

    public Transform turret;
    public float rotSpeed;

    public Transform firePos;

    public float HP;

    public enum TurretDirection
    {
        None,
        Top,
        LTop,
        RTop,
        Left,
        Right,
        LBottom,
        RBottom,
        Bottom,
        End,
    }

    private void Awake()
    {
        Player = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(Vector3.forward * moveSpeed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            rigidbody.AddForce(Vector3.back * moveSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddForce(Vector3.left * moveSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.AddForce(Vector3.right * moveSpeed);
        }

        if(Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }

        bool IsUp = false;
        bool IsDown = false;
        bool IsLeft = false;
        bool IsRight = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            IsUp = true;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            IsDown = true;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            IsLeft = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            IsRight = true;
        }

        TurretDirection dir = TurretDirection.None;
        if (IsUp == true)
        {
            if (IsLeft == true)
            {
                dir = TurretDirection.LTop;
            }
            else if (IsRight == true)
            {
                dir = TurretDirection.RTop;
            }
            else
            {
                dir = TurretDirection.Top;
            }
        }
        else if(IsDown == true)
        {
            if (IsLeft == true)
            {
                dir = TurretDirection.LBottom;
            }
            else if (IsRight == true)
            {
                dir = TurretDirection.RBottom;
            }
            else
            {
                dir = TurretDirection.Bottom;
            }
        }
        else
        {
            if (IsLeft == true)
            {
                dir = TurretDirection.Left;
            }
            else if (IsRight == true)
            {
                dir = TurretDirection.Right;
            }
            else
            {
                dir = TurretDirection.Top;
            }
           
        }

        TurretRot(dir);
    }

    void TurretRot(TurretDirection dir)
    {
        float targetAngle = 0.0f;

        switch(dir)
        {
            case TurretDirection.Top:
            {
                    targetAngle = 0;

            }
            break;

            case TurretDirection.RTop:
                {
                    targetAngle = 45;

                }
                break;

            case TurretDirection.Right:
                {
                    targetAngle = 90;

                }
                break;

            case TurretDirection.RBottom:
                {
                    targetAngle = 135;

                }
                break;

            case TurretDirection.Bottom:
                {
                    targetAngle = 180;

                }
                break;

            case TurretDirection.LBottom:
                {
                    targetAngle = 225;

                }
                break;

            case TurretDirection.Left:
                {
                    targetAngle = 270;

                }
                break;

            case TurretDirection.LTop:
                {
                    targetAngle = 315;

                }
                break;            
        }
        Quaternion qat = Quaternion.Euler(0, targetAngle, 0);
        turret.rotation = Quaternion.Lerp(turret.rotation, qat, 0.8f);
    }

    void Shoot()
    {
        GameObject bullet = EffectManager.Instance.GetEffect(EffectType.BulletA_Projectile);
        bullet.SetActive(true);

        BulletBase bulletBase = bullet.GetComponent<BulletBase>();
        if (bulletBase == null)
        {
            bulletBase = bullet.AddComponent<BulletBase>();            
        }

        bulletBase.Init(firePos.position, turret.rotation, 100, 10, 20, BulletTargetType.ToEnemy);
    }

    public void Damage(float damage)
    {
        if(HP - damage <= 0)
        {
            GameObject effect = EffectManager.Instance.GetEffect(EffectType.Explosion_A);
            effect.transform.position = transform.position;
            effect.SetActive(true);

            gameObject.SetActive(false);
            HP = 0;
            return;
        }

        HP -= damage;
    }
}
