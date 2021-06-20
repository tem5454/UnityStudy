using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyBase : MonoBehaviour
{
    public float currentHP = 0.0f;
    public float maxHP = 0.0f;
    public Color startColor;
    public Color endColor;

    public Vector3 deadPos;

    public void Start()
    {
        deadPos = transform.position + new Vector3(0, 10, 0);
        SetColor();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            BulletBase bullet = collision.gameObject.GetComponent<BulletBase>();
            currentHP -= bullet.damage;

            SetColor();
        }
    }

    public void Update()
    {
        if (currentHP <= 0.0f)
        {
            transform.position = Vector3.Lerp(transform.position, deadPos, Time.deltaTime);
        }
    }

    public void SetColor()
    {
        Renderer render = GetComponent<Renderer>();
        render.material.color = Color.Lerp(startColor, endColor, currentHP / maxHP);
    }
}
