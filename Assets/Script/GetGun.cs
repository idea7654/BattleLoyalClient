using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGun : MonoBehaviour
{
    Animator animator;
    NetworkManager networkManager;
    //public GameObject[] Guns;

    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        //Guns = GameObject.FindGameObjectsWithTag("Gun");
    }
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
            if (hitInfo.transform.tag == "Gun")
            {
                //Debug.Log("발견!!");
                //여기서 총 주울 수 있도록...
                //hitInfo.transform.gameObject.GetComponent<Rigidbody>().enabled = false;
                Destroy(hitInfo.transform.gameObject.GetComponent<Rigidbody>());
                hitInfo.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                
                int gunNum = hitInfo.transform.GetChild(0).transform.gameObject.GetComponent<Gun>().gunNum;
                var packet = networkManager.WritePacketManager.WRITE_PU_C2S_PICKUP_GUN(networkManager.MyNick, gunNum);
                networkManager.SendPacket(packet);
            }
        }
    }

    public void PickGun(int gunNum)
    {
        networkManager.SessionGuns.ForEach(delegate (GameObject gun)
        {
            if (gunNum == gun.transform.GetChild(0).transform.gameObject.GetComponent<Gun>().gunNum)
            {
                Destroy(gun.transform.gameObject.GetComponent<Rigidbody>());
                gun.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                //GameObject targetUserHand = RecursiveFindChild(transform, "RightHandAttatch");
                Transform targetUserHand = RecursiveFindChild(transform, "RightHandAttatch");
                LateUpdatedFollow lateUpdatedFollow = gun.transform.gameObject.GetComponent<LateUpdatedFollow>();
                lateUpdatedFollow.SetTarget(targetUserHand); //이거 다른플레이어들도 되도록
                //lateUpdatedFollow.target = targetUserHand;
                PlayerShooter Ps = transform.GetChild(0).GetComponent<PlayerShooter>();
                Ps.target = gun.transform.GetChild(1).transform;
                animator.SetBool("hasGun", true);
            }
        });
    }

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        Transform child = null;
        for (int i = 0; i < parent.childCount; i++)
        {
            child = parent.GetChild(i);
            if (child.name == childName)
            {
                break;
            }
            else
            {
                child = RecursiveFindChild(child, childName);
                if (child != null)
                {
                    break;
                }
            }
        }

        return child;
    }
}