using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f; //총마다 다르게..
    public float range = 100f; //총마다 다르게..
    private float fireRate = 1f;
    //public Camera tpsCam;
    //public Transform firePoint;

    private float timer;

    private Transform characterBody;
    private NetworkManager networkManager;
    Animator animator;

    void Start()
    {
        //networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //characterBody = GameObject.Find(networkManager.MyNick).transform.GetChild(0);
        characterBody = GameObject.Find("Nickname").transform;
        animator = characterBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > fireRate)
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
                timer = 0f;
                animator.SetBool("isShoot", true);
            }
        }
        else
        {
            animator.SetBool("isShoot", false);
        }
    }
    
    private void Shoot()
    {
        /*
        RaycastHit hit;
        if (Physics.Raycast(tpsCam.transform.position, tpsCam.transform.forward, out hit))
        {
            //Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }
        }
        */

        Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);
        //Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, 100))
        {
            Debug.Log(hitInfo.transform.name);
            //여기에 체력닳는 등 처리..
        }
    }
}
