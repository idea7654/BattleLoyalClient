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
            if(hitInfo.transform.tag == "Gun")
            {
                Debug.Log("발견!!");
                //여기서 총 주울 수 있도록...
                //hitInfo.transform.gameObject.GetComponent<Rigidbody>().enabled = false;
                //Destroy(hitInfo.transform.gameObject.GetComponent<Rigidbody>());
                //hitInfo.transform.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                //int gunNum = hitInfo.transform.GetChild(0).transform.gameObject.GetComponent<Gun>().gunNum;
                //var packet = networkManager.WritePacketManager.WRITE_PU_C2S_PICKUP_GUN(networkManager.MyNick, gunNum);
                //networkManager.SendPacket(packet);
            }
        }
    }

    public void PickGun(int gunNum)
    {
        Debug.Log(networkManager.GunList.Count);
        //networkManager.GunList.ForEach(delegate (GameObject gun)
        //{
        //    if (gunNum == gun.transform.GetChild(0).transform.gameObject.GetComponent<Gun>().gunNum)
        //    {
        //        gun.transform.gameObject.GetComponent<LateUpdatedFollow>().targetToFollow = GameObject.Find("RightHandAttatch").transform; //이거 다른플레이어들도 되도록
        //        PlayerShooter Ps = GameObject.Find(networkManager.MyNick).GetComponent<PlayerShooter>();
        //        Ps.target = gun.transform.GetChild(1).transform;
        //        Debug.Log(Ps.target);
        //        animator.SetBool("hasGun", true);
        //    }
        //});
    }
}
