using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperCharacter : MonoBehaviour
{
    public bool IsZoom;
    public Cinemachine.CinemachineVirtualCamera NormalCamera;
    public Cinemachine.CinemachineVirtualCamera ZoomCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            IsZoom = !IsZoom;
            ZoomInOut();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }
        Debug.DrawLine(ZoomCamera.transform.position, ZoomCamera.transform.forward + new Vector3(0, 0, 100), Color.red);
        
    }

    void Fire()
    {
        GameObject obj = EffectManager.Instance.GetEffect(EffectType.BulletA_Projectile);
        if(obj)
        {
            obj.transform.position = ZoomCamera.transform.position;
            obj.transform.rotation = ZoomCamera.transform.rotation;

            BulletBase bullet = obj.GetComponent<BulletBase>();
            if(bullet)
            {
                bullet.Init(ZoomCamera.transform.position, ZoomCamera.transform.rotation, 100, 10, 10, BulletTargetType.None);
            }
            obj.gameObject.SetActive(true);

            Ray ray = new Ray(ZoomCamera.transform.position, ZoomCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f))
            {
                Renderer render = hitInfo.transform.gameObject.GetComponent<Renderer>();
                if(render)
                {
                    float r = Random.Range(0, 255.0f) / 255.0f;
                    float g = Random.Range(0, 255.0f) / 255.0f;
                    float b = Random.Range(0, 255.0f) / 255.0f;

                    render.material.color = new Color(r, g, b);

                    Debug.Log($"RGB : {r} {g} {b}");
                }
            }
            
        }
    }

    void ZoomInOut()
    {
        ZoomCamera.gameObject.SetActive(IsZoom);
        NormalCamera.gameObject.SetActive(!IsZoom);
    }
}
