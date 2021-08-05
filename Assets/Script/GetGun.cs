using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGun : MonoBehaviour
{
    Animator animator;
    // Update is called once per frame
    void Update()
    {
        SearchTarget();
    }

    private void SearchTarget()
    {
        Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 100))
        {
            if(hitInfo.transform.tag == "Gun")
            {
                Debug.Log("총 발견!!");
                //여기서 총 주울 수 있도록...
                //hitInfo.transform.gameObject.GetComponent<Rigidbody>().enabled = false;
                Destroy(hitInfo.transform.gameObject.GetComponent<Rigidbody>());
                hitInfo.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                hitInfo.transform.gameObject.GetComponent<LateUpdatedFollow>().targetToFollow = GameObject.Find("RightHandAttatch").transform;
                PlayerShooter Ps = GameObject.Find("Nickname").GetComponent<PlayerShooter>();
                Ps.target = hitInfo.transform.GetChild(1).transform;
                animator = transform.GetChild(0).GetComponent<Animator>();
                animator.SetBool("hasGun", true);
            }
        }
    }
}
