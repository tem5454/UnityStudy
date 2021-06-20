using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 3.0f;
    public float roSpeed = 0.0f;
    //public GameObject bulletOrigin;
    //public Transform firePos;
    public Animator animator;

    public void Update()
    {
        // Input.GetKey 누르고 있으면 계속 호출
        // Input.GetKeyDown 1번 호출

        bool IsMove = false;
        bool IsRun = false;

        Vector3 nextPos = new Vector3();        
        Vector3 roVec = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                
        if (Input.GetKey(KeyCode.W))
        {
            nextPos = transform.forward * Time.unscaledDeltaTime * speed;
            IsMove = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            nextPos = (transform.forward * -1) * Time.unscaledDeltaTime * speed;
            IsMove = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            //moveVec.x -= 1;
            roVec.y -= (1 * roSpeed * Time.unscaledDeltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            //moveVec.x += 1;
            roVec.y += (1 * roSpeed * Time.unscaledDeltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Time.timeScale = 1.0f;
            animator.SetFloat("AnimSpeed", 1.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Time.timeScale = 2.0f;
            animator.SetFloat("AnimSpeed", 2.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Time.timeScale = 0.30f;
            animator.SetFloat("AnimSpeed", 0.5f);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            IsRun = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("HotTrigger");
        }

        if (IsMove)
        {
            transform.localPosition += nextPos;
            if(IsRun)
            {
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsRun", true);
                Debug.Log("IsRun true");
            }
            else
            {
                animator.SetBool("IsWalk", true);
                animator.SetBool("IsRun", false);
                Debug.Log("IsRun false");
            }
            
        }
        else
        {
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsRun", false);
        }

        transform.rotation = Quaternion.Euler(roVec);         
    }

    public void Shoot()
    {
        //GameObject newBullet = Instantiate(bulletOrigin);
        //newBullet.transform.position = firePos.position;

        //BulletBase bullet = newBullet.GetComponent<BulletBase>();
        //bullet.Init(firePos.position, transform.rotation, 50.0f, 10.0f, 60.0f);

        //newBullet.SetActive(true);
    }
}
