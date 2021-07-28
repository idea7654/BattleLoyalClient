using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f; //총마다 다르게..
    public float range = 100f; //총마다 다르게..
    public ParticleSystem particle;
    public Camera tpsCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        particle.Play();
        RaycastHit hit;
        if (Physics.Raycast(tpsCam.transform.position, tpsCam.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
